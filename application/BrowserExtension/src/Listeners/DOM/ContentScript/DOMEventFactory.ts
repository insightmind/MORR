import * as Shared from '../../../Shared/SharedDeclarations'
import * as DOM from '../DOMEvents'

export default class DOMEventFactory {
    //maximum length for target string. set to zero to remove limit
    private static readonly TARGETMAXLENGTH = 256;
    //matcher to check if a string contains only whitespaces
    private static readonly EMPTYMATCHER = new RegExp('^\ *$');
    /*
     * Filters for ButtonClickEvent (attempting to avoid event spam when clicking on textareas).
     * There seems to be no elegant solution to this problem, as there is no way of telling if something is a button
     * or if something has an onclick-listener attached to it.
     */
    //a (href) and button elements are the common tags for button-like objects, but also input-type:submit.
    private static readonly BUTTONCLICK_WHITELISTFILTER = new RegExp('^(a|button|input)$', 'i');
    private static readonly BUTTONCLICK_INPUTTYPEWHITELISTFILTER = new RegExp('^(submit|checkbox|button|radio)$', 'i');
    //define for which elements hover-events shall be suppressed (e.g. hovering over a whole paragraph usually does not matter)
    private static readonly HOVER_BLACKLISTFILTER = new RegExp('^(p|body|head|div|html|ul)$', 'i');
    //textinput causes the 'change' event, which generally appears on 'input' elements, which can have many different types
    private static readonly TEXTINPUT_WHITELISTFILTER = new RegExp('^(textarea|input)$', 'i');
    private static readonly TEXTINPUT_ELEMENTTYPEWHITELISTFILTER = new RegExp('^(text)$', 'i');

    //helper variable to prevent misfires of textselection-events. It declared here since there are no static method variables.
    lastTextSelection : string = "";

    /**
     * Creates (MORR-)Event based on DOM Event type
     * @param domEvent 
     * @returns event 
     */
    public createEvent(domEvent : Event) : Promise<Shared.BrowserEvent | undefined> {
        return new Promise((resolve) => {
            let evt : Shared.BrowserEvent | undefined;
            switch(domEvent.type) {
                case(DOM.DOMEventTypes.DBLCLICK):
                case(DOM.DOMEventTypes.CLICK):
                    evt = this.createButtonClickEvent(domEvent as MouseEvent);
                    resolve(evt);
                    break;
                case(DOM.DOMEventTypes.KEYUP):
                case(DOM.DOMEventTypes.MOUSEUP):
                    evt = this.createTextSelectionEvent(domEvent);
                    resolve(evt);
                    break;
                case(DOM.DOMEventTypes.CHANGE):
                    evt = this.createTextInputEvent(domEvent);
                    resolve(evt);
                    break;
                case(DOM.DOMEventTypes.MOUSEENTER):
                    this.createHoverEvent(domEvent)
                    .then((evt) => resolve(evt));
                    break;
            }
        });
    }

    /**
     * Creates button click event
     * @param domEvent 
     * @returns button click event 
     */
    private createButtonClickEvent(domEvent : MouseEvent) : DOM.ButtonClickEvent | undefined {
        let target : HTMLElement | null = <HTMLElement>domEvent.target;
        //search the hierarchy upwards to see if the target is part of a whitelisted element
        while (target) {
            if (DOMEventFactory.BUTTONCLICK_WHITELISTFILTER.test(target.tagName)) {
                if (target.tagName.toLowerCase() == "input" && !DOMEventFactory.BUTTONCLICK_INPUTTYPEWHITELISTFILTER.test((<HTMLInputElement>target).type))
                    return undefined;
                let buttonHref : string | null = target.getAttribute("href");
                //unlike in other event types, honor the innertext of the button should have priority over its ID etc
                let buttonTitle = DOMEventFactory.extractContent(target.outerHTML);
                if (DOMEventFactory.EMPTYMATCHER.test(buttonTitle)) {
                    if (target.tagName.toLowerCase() == "input" && (<HTMLInputElement>target).value.length > 0)
                        buttonTitle = (<HTMLInputElement>target).value;
                    else
                        buttonTitle = DOMEventFactory.extractTargetAsString(target);
                }
                return new DOM.ButtonClickEvent(0, 0, buttonTitle, window.location.href,
                                                buttonHref ? buttonHref : undefined);
            }
            target = target.parentElement;
        }
        //neither the target nor any of its parent were whitelisted
        return undefined;
    }

    /**
     * Creates text input event
     * @param domEvent 
     * @returns text input event 
     */
    private createTextInputEvent(domEvent : Event) : DOM.TextInputEvent | undefined {
        if (domEvent.target && DOMEventFactory.TEXTINPUT_WHITELISTFILTER.test((<HTMLElement>domEvent.target).tagName)) {
            const target = domEvent.target as HTMLInputElement;
            //input-elements could also have type radio, button etc...
            //only allowing text also effectively also filters out password-inputs, as those fields have type 'password'
            if (target.tagName.toLowerCase() == "input" && !DOMEventFactory.TEXTINPUT_ELEMENTTYPEWHITELISTFILTER.test(target.type))
                return undefined;
            let inputText : string = target.value;
            return new DOM.TextInputEvent(0, 0, inputText, DOMEventFactory.extractTargetAsString(target), window.location.href);
        }
        return undefined;
    }

    /**
     * Creates text selection event
     * @param domEvent 
     * @returns text selection event 
     */
    private createTextSelectionEvent(domEvent : Event) : DOM.TextSelectionEvent | undefined {
        if (domEvent.type == DOM.DOMEventTypes.KEYUP) {
            if ((<KeyboardEvent>domEvent).key != "Control" && (<KeyboardEvent>domEvent).key != "Shift")
                return;
        }
        if (window && window.getSelection()) {
            let textSelection : Selection | null = window.getSelection();
            if (textSelection && textSelection.toString() != this.lastTextSelection) {
                this.lastTextSelection = textSelection.toString();
                if (this.lastTextSelection.length > 0)
                    return new DOM.TextSelectionEvent(0, 0, this.lastTextSelection.toString(), window.location.href);
            }
        }
        return undefined;
    }

    /**
     * Create a HoverEvent if the cursor does not leave the element for HOVERDELAYMS seconds
     * @param domEvent
     * @returns hover event or undefined if not applicable
     */
    private createHoverEvent(domEvent : Event) : Promise<DOM.HoverEvent | undefined> {
        return new Promise((resolve) => {
            let url : string = window.location.href;
            //apply blacklist filtering
            if (!domEvent.target || DOMEventFactory.HOVER_BLACKLISTFILTER.test((<HTMLElement>domEvent.target).tagName))
                resolve(undefined);
            let valid : boolean = true;
            let invalidate = () => {valid = false;}
            document.addEventListener("mouseout", invalidate, {
                capture : true,
                passive : true,
                once : true
            });
            setTimeout(() => {
                document.removeEventListener("mouseout", invalidate, {capture : true});
                if (!valid)
                    resolve(undefined);
                else
                    resolve(new DOM.HoverEvent(0, 0, DOMEventFactory.extractTargetAsString(<HTMLElement>domEvent.target), url));
            }, DOM.HoverEvent.HOVERDELAYMS);
        });
    }
 
    /**
     * Extracts target as string from Event
     * @param domEvent the domEvent to extract the target from
     * @returns string describing the target (label of button, etc.)
     */
    private static extractTargetAsString(target : HTMLElement) : string {
        let targetString : string = "";
        /**
         * Use the first value that's available from list:
         * [id, name, title, aria-label, innertext, value, outerHTML]
         */
        if (target.id && target.id.length > 0)
            targetString = target.id;
        else if((<HTMLButtonElement>target).name && (<HTMLButtonElement>target).name.length > 0)
            targetString = (<HTMLButtonElement>target).name;
        else if (target.title && target.title.length > 0)
            targetString = target.title;
        else if (target.hasAttribute("aria-label") && target.getAttribute("aria-label") && target.getAttribute("aria-label")!.length > 0) {
               targetString = target.getAttribute("aria-label")!;
        } else {
            targetString = DOMEventFactory.extractContent(target.outerHTML);
            if (DOMEventFactory.EMPTYMATCHER.test(targetString)) {
                if ((<HTMLButtonElement>target).value && (<HTMLButtonElement>target).value.length > 0)
                    targetString = (<HTMLButtonElement>target).value;
                else
                    targetString = target.outerHTML; //fallback if everything else fails
            }
        }

        //truncate if set
        if (DOMEventFactory.TARGETMAXLENGTH > 0 && targetString.length > DOMEventFactory.TARGETMAXLENGTH)
            targetString = targetString.substr(0, DOMEventFactory.TARGETMAXLENGTH) + "...<trunc>";
        return targetString;
    }

    //try to get retrive a meaningful name from the (outer)HTML of the element
    private static extractContent(s : any, space : boolean = true) : string {
        var span= document.createElement('span');
        span.innerHTML = s;
        if (!span || !span.innerText) //check if creating span failed
            return "";
        //replace excessive whitespaces and newlines before returning
        return span.innerText.toString().replace(/ +/g,' ').replace(/\ *$/, '').replace(/(\n\ *\t*)+/g, '\n');
    };
}
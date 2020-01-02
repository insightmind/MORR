import * as Shared from '../../../Shared/SharedDeclarations'
import * as DOM from '../DOMEvents'
interface ParsedEvent {
    type : Shared.EventType
}

export default class DOMEventFactory {
    //matcher to filter out password inputs
    private static readonly passwordMatcher = new RegExp('password', 'i');
    //matcher to check if a string contains only whitespaces
    private static readonly emptyMatcher = new RegExp('^\ *$');
    /*
     * Filters for ButtonClickEvent (attempting to avoid event spam when clicking on textareas).
     * There seems to be no elegant solution to this problem, as there is no way of telling if something is a button
     * or if something has an onclick-listener attached to it.
     */
    //p, span and div are usually not used as buttons.
    private static readonly targetBlackListFilter = new RegExp('^(p|span|div)$', 'i');
    //a (href) and button elements are the common tags for button-like objects.
    private static readonly targetWhiteListFilter = new RegExp('^(a|button|input)$', 'i');
    private static readonly inputTypeWhiteListFilter = new RegExp('^(submit|checkbox|button)$', 'i');
    lastTextSelection : string = "";
    /**
     * Creates event
     * @param domEvent 
     * @returns event 
     */
    public createEvent(domEvent : Event) : Shared.BrowserEvent | undefined {
        let evt : Shared.BrowserEvent | undefined;
        switch(domEvent.type) {
            case(DOM.DOMEventTypes.CLICK):
                evt = this.createButtonClickEvent(domEvent as MouseEvent);
                break;
            case(DOM.DOMEventTypes.MOUSEUP):
                evt = this.createTextSelectionEvent(domEvent);
                break;
            case(DOM.DOMEventTypes.CHANGE):
                evt = this.createTextInputEvent(domEvent);
                break;
        }
        return evt;
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
            if (DOMEventFactory.targetWhiteListFilter.test(target.tagName)) {
                if (target.tagName.toLowerCase() == "input" && !DOMEventFactory.inputTypeWhiteListFilter.test((<HTMLInputElement>target).type))
                    return undefined;
                let buttonHref : string | null = target.getAttribute("href");
                return new DOM.ButtonClickEvent(0, 0, DOMEventFactory.extractTargetAsString(domEvent), window.location.href,
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
        if (domEvent.target) {
            const target = domEvent.target as HTMLInputElement;
            let inputText : string;
            let targetString = DOMEventFactory.extractTargetAsString(domEvent);
            if (DOMEventFactory.passwordMatcher.test(target.type) || DOMEventFactory.passwordMatcher.test(targetString))
                inputText = "<password>";
            else {
                inputText = target.value;
            }
            return new DOM.TextInputEvent(0, 0, inputText, DOMEventFactory.extractTargetAsString(domEvent), window.location.href);
        }
        return undefined;
    }

    /**
     * Creates text selection event
     * @param domEvent 
     * @returns text selection event 
     */
    private createTextSelectionEvent(domEvent : Event) : DOM.TextSelectionEvent | undefined {
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
     * Extracts target as string from Event
     * @param domEvent the domEvent to extract the target from
     * @returns string describing the target (label of button, etc.)
     */
    private static extractTargetAsString(domEvent : Event) : string {
        const target = <Element>domEvent.target;
        let targetString : string = DOMEventFactory.extractContent(target.outerHTML);
        if (DOMEventFactory.emptyMatcher.test(targetString)) {
            if ((<HTMLButtonElement>target).name)
            targetString = (<HTMLButtonElement>target).name;
            else if ((<HTMLButtonElement>target).value)
                targetString = (<HTMLButtonElement>target).value;
        }
        if (targetString.length == 0)
            targetString = target.outerHTML; //fallback if everything else fails
        return targetString;
    }
    //TODO: replace this copy-paste
    private static extractContent(s : any, space : boolean = true) : string {
        var span= document.createElement('span');
        span.innerHTML= s;
        if(space) {
          var children : any = span.querySelectorAll('*');
          for(var i = 0 ; i < children.length ; i++) {
            if(children[i].textContent)
              children[i].textContent+= ' ';
            else
              children[i].innerText+= ' ';
          }
        }
        return [span.textContent || span.innerText].toString().replace(/ +/g,' ').replace(/\ *$/, '').replace(/(\n\ *\t*)+/g, '\n');
      };
}
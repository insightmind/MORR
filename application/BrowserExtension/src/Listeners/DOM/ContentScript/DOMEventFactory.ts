import * as Shared from '../../../Shared/SharedDeclarations'
import * as DOM from '../DOMEvents'
interface ParsedEvent {
    type : Shared.EventType
}

export default class DOMEventFactory {
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
            /*case(DOM.DOMEventTypes.MOUSEDOWN):
                evt = this.createTextSelectionEvent(domEvent);
                break;*/
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
    public createButtonClickEvent(domEvent : MouseEvent) : DOM.ButtonClickEvent | undefined {
        let text : string = this.extractContent((<Element>domEvent.target).outerHTML);
        let buttonHref : string | null = (<Element>domEvent.target).getAttribute("href");
        return new DOM.ButtonClickEvent(0, 0, text, window.location.href, buttonHref ? buttonHref : undefined);
    }

    /**
     * Creates text input event
     * @param domEvent 
     * @returns text input event 
     */
    public createTextInputEvent(domEvent : Event) : DOM.TextInputEvent | undefined {
        return undefined;
    }

    /**
     * Creates text selection event
     * @param domEvent 
     * @returns text selection event 
     */
    public createTextSelectionEvent(domEvent : Event) : DOM.TextSelectionEvent | undefined {
        return undefined;
    }

    //TODO: replace this copy-paste
    private extractContent(s : any, space : boolean = true) : string {
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
        return [span.textContent || span.innerText].toString().replace(/ +/g,' ');
      };
}
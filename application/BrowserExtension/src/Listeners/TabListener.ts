import { IListener, EventType } from '../Shared/SharedDeclarations'
import { BrowserEvent } from '../Shared/SharedDeclarations'
export default class TabListener implements IListener {
    constructor(callback: (event: BrowserEvent) => void) {

    }
    start(): void {
        throw new Error("Method not implemented.")
    }    stop(): void {
        throw new Error("Method not implemented.")
    }
}
import { lookup } from "./entities";

export interface createCollaboratorView {
    requestStatus: lookup;
    collaboratorType: lookup;
    isOwner: boolean;
}
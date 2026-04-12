export interface collaboration {
    id?: number;
    projectId?: number;
    project?: object;
    userId?: number;
    user?: object;
    requestStatusId?: number;
    requestStatus?: lookup;
    collaboratorTypeId?: number;
    collaboratorType?: lookup;
    joinedAt: Date,
    isOwner: boolean;
}

export interface lookup {
    id: number;
    name: string;
}
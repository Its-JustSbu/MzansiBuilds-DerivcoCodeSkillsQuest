export interface collaboration {
    id?: number;
    projectId?: number;
    project?: project;
    userId?: number;
    user?: user;
    requestStatusId?: number;
    requestStatus?: lookup;
    collaboratorTypeId?: number;
    collaboratorType?: lookup;
    joinedAt?: Date,
    isOwner: boolean;
}

export interface milestone {
    id?: number;
    description: string;
    projectStageId?: number;
    projectStage?: projectStage;
    modifiedAt: Date;
}

export interface projectStage {
    id?: number;
    stageNumber: number;
    stageTitle: string;
    modifiedAt: Date;
    stageStatusId?: number;
    stageStatus?: lookup;
    projectId?: number;
    project?: object;
    milestones?: milestone[]; 
}

export interface project {
    id?: number;
    name: string;
    description: string;
    createdAt: Date;
    stages?: projectStage[];
    support?: support[];
    collaborations?: collaboration[];
    comments?: comment[];
}

export interface support {
    id?: number;
    description: string;
    supportTypeId?: number;
    supportType?: lookup;
    projectId?: number;
    project?: project;
    requestedAt: Date;
}

export interface comment {
    id?: number;
    projectId?: number;
    project?: project;
    userId?: number;
    user?: user;
    title: string;
    description: string;
    createdAt: Date;
}

export interface user {
    id?: number;
    name: string;
    surname:string;
    emailAddress: string;
    username: string;
    password?: string;
    salt?: string;
    createdAt: Date;
    modifiedAt: Date;
}

export interface refreshToken {
    id?: number;
    token: string;
    expiredAt: Date;
    isValid: Date;
    userId?: number;
    user?: user;
}

export interface lookup {
    id: number;
    name: string;
}
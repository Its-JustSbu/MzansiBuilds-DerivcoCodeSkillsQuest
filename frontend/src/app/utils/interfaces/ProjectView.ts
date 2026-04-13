import { lookup, projectStage } from "./entities";

export interface ProjectViewDTO {
    name: string;
    description: string;
    stages?: StageViewDTO[];
    support?: SupportViewDTO[];
}

export interface StageViewDTO {
    stageTitle: string;
    stageNumber: number;
    stageStatus: lookup;
    milestones: MilestoneViewDTO[]
}

export interface MilestoneViewDTO {
    description: string;
    projectStage?: projectStage;
}

export interface SupportViewDTO {
    description: string;
    supportType: lookup;
}
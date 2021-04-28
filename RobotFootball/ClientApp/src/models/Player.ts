import { GameObject } from ".";

export interface Player extends GameObject {
    playerId: number;
    isOpponent: boolean;
}

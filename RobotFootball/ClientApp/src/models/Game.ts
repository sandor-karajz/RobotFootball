import { GameObject, Player } from ".";
import { GameStatus } from "../enums";

export interface Game {
    playerIndex: number;
    status: GameStatus;
    playerScores: number[];
    ball: GameObject;
    players: Player[];
}
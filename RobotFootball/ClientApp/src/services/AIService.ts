import Utilities from "../utilities/Utilities";
import { Command, Game, Player, Vector } from "../models";
import { GameStatus } from "../enums";
import { AREAS, GOAL_LINE_LENGHT, TOUCHLINE_LENGHT } from "../constants";

export default class AIService {
    static getCommands(game: Game): Command[] {
        if (game.status !== GameStatus.InProgress) {
            return [];
        }

        const ballPosition = game.ball.position;
        const shouldFlip = game.playerIndex === 1;
        const areaRatio = this.getBallAreaRatio(ballPosition, shouldFlip);
        const playerPositions = this.getPlayerPositions(areaRatio);
        const ownPlayers = game.players.filter(x => !x.isOpponent);
        const closestPlayer = this.getClosestPlayerToBall(ownPlayers, ballPosition);


        return ownPlayers.map(x => {
            const isClosest = this.isClosestPlayer(x, closestPlayer);

            return {
                playerId: x.playerId,
                targetX: isClosest ? ballPosition.x : this.flip(playerPositions[x.playerId].x, TOUCHLINE_LENGHT, shouldFlip),
                targetY: isClosest ? ballPosition.y : this.flip(playerPositions[x.playerId].y, GOAL_LINE_LENGHT, shouldFlip),
                isShooting: isClosest
            };
        });
    }

    private static getBallAreaRatio(ballPosition: Vector, shouldFlip: boolean): Vector {
        return {
            x: (shouldFlip ? TOUCHLINE_LENGHT - ballPosition.x : ballPosition.x) / TOUCHLINE_LENGHT,
            y: ballPosition.y / GOAL_LINE_LENGHT
        };
    }

    private static getPlayerPositions(areaRatio: Vector): Vector[] {
        return AREAS.map(x => ({
            x: this.getPositionFromArea(x.bottomLeft.x, x.topRight.x, areaRatio.x),
            y: this.getPositionFromArea(x.bottomLeft.y, x.topRight.y, areaRatio.x)
        }));
    }

    private static getClosestPlayerToBall(players: Player[], ballPosition: Vector) {
        return players.sort((a, b) => Utilities.getDistance(a.position, ballPosition) - Utilities.getDistance(b.position, ballPosition))[0];
    }

    private static isClosestPlayer(player: Player, closestPlayer: Player) {
        return player.playerId === closestPlayer.playerId;
    }

    private static flip(value: number, max: number, shouldFlip: boolean) {
        return shouldFlip ? max - value : value;
    }

    private static getPositionFromArea(bottomLeft: number, topRight: number, ratio: number) {
        return bottomLeft + (topRight - bottomLeft) * ratio;
    }
}
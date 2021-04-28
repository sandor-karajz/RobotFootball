import { Component } from 'react';
import { Vector } from '../models';

export default class Utilities {
    static getSetStateAsync = <P, S>(instance: Component<P, S>) => <K extends keyof S>(stateChange: ((prevState: Readonly<S>, props: Readonly<P>) => (Pick<S, K> | S | null)) | (Pick<S, K> | S | null)) =>
        new Promise<void>(resolve => instance.setState(stateChange, resolve));

    static getDistance(vector1: Vector, vector2: Vector) {
        const distanceX = vector1.x - vector2.x;
        const distanceY = vector1.y - vector2.y;

        return Math.sqrt(Math.pow(distanceX, 2) + Math.pow(distanceY, 2));
    }

    
}

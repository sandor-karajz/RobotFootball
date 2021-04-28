import React from 'react';
import { Route } from 'react-router';
import RobotFootballManager from './components/RobotFootballManager';

export default () => (
    <Route exact path='/' component={RobotFootballManager} />
);

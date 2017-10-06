import React from "react";
import {BrowserRouter as Router, Route} from 'react-router-dom';
import Trips from "./Trips.jsx";
import Trip from "./Trip.jsx";

export default function(props) {
    return (
        <Router>
            <div>
                <Route exact path="/" render={_ => <Trips trips={props.trips}/>}/>
                <Route path="/client/trips/:id" render={({match}) => <Trip id={match.params.id}/>}/>
            </div>
        </Router>
    );
}

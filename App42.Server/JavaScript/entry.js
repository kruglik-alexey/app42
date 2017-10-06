import "babel-polyfill";
import "bootstrap/dist/css/bootstrap.min.css";
import React from "react";
import ReactDOM from "react-dom";
import App from "./App.jsx";

const getEvents = function(tripId, afterEvent = null, type = "l") {
    return fetch(`trips/${tripId}/events`).then(r => r.json());
};

const mapReady = new Promise(r => {
    const loader = () => {
        setTimeout(() => {
            if (!window.google) {
                loader();
            } else {
                r();
            }
        }, 0);
    };
    loader();
});

const toGMapsCoords = c => new google.maps.LatLng({lat: c.lat, lng: c.lon});

const renderEvents = (map, line, evs) => {
    evs.map(toGMapsCoords).forEach(x => line.getPath().push(x));
    map.setCenter(toGMapsCoords(evs[evs.length - 1]));
};

mapReady.then(() => fetch("/trips")).then(r => r.json()).then(trips => {
    ReactDOM.render(React.createElement(App, { trips }), document.getElementById("root"));
    /*const map = new google.maps.Map(document.getElementById('root'), {
        zoom: 15,
        center: {lat: -25.363, lng: 131.044}
    });

    const line = new google.maps.Polyline({
        map,
        path: [],
        geodesic: true,
        strokeColor: '#FF0000',
        strokeOpacity: 1.0,
        strokeWeight: 2
    });

    getEvents(trip.id).then(evs => renderEvents(map, line, evs));*/
});

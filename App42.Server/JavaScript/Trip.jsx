import React from "react";

const refreshInterval = process.env === 'PRODUCTION' ? 30000 : 5000;

const fetchEvents = (tripId, afterEventId) => {
    return fetch(`/trips/${tripId}/events/${afterEventId || ""}`).then(r => r.json());
};

const toGMapsCoords = c => new google.maps.LatLng({lat: c.lat, lng: c.lon});

export default class Trip extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            lastRefreshTime: null,
            lastEventTime: null,
            lastEventId: null
        };
        this.refreshing = false;
        this.unmounted = false;
    }

    componentDidMount() {
        this.map = new google.maps.Map(this.mapContainer, {
            zoom: 15,
            center: {lat: -25.363, lng: 131.044}
        });

        this.line = new google.maps.Polyline({
            map: this.map,
            path: [],
            geodesic: true,
            strokeColor: '#FF0000',
            strokeOpacity: 1.0,
            strokeWeight: 2
        });

        this.timer = setInterval(this.refresh.bind(this), refreshInterval);
        this.refresh();
    }

    componentWillUnmount() {
        if (this.timer) {
            clearInterval(this.timer);
        }
        this.unmounted = true;
    }

    render() {
        return (
            <div>
                <div>
                    Last refresh: <span>{this.state.lastRefreshTime && this.state.lastRefreshTime.toLocaleString()}</span>
                </div>
                <div>
                    Last event: <span>{this.state.lastEventTime && this.state.lastEventTime.toLocaleString()}</span>
                </div>

                <div className="map" ref={r => this.mapContainer = r}></div>
            </div>
        );
    }

    refresh() {
        if (this.refreshing || this.unmounted) {
            return;
        }

        fetchEvents(this.props.id, this.state.lastEventId).then(evs => {
            this.setState({lastRefreshTime: new Date()});
            if (evs.length === 0) {
                return;
            }

            const lastEventId = evs.reduce((acc, e) => Math.max(acc, e.id), 0);
            this.setState({lastEventId});

            const lastEventTime = evs.map(e => new Date(e.date)).reduce((acc, d) => d > acc || !acc ? d : acc);
            this.setState({lastEventTime});

            const gmapsCoords = evs.map(toGMapsCoords);
            if (this.line.getPath().length === 0) {
                const bounds = new google.maps.LatLngBounds();
                gmapsCoords.forEach(c => bounds.extend(c));
                this.map.fitBounds(bounds);
            }
            gmapsCoords.forEach(x => this.line.getPath().push(x));
        });
    }
}

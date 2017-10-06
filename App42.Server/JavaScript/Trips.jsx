import React from "react";
import {Link} from 'react-router-dom';

const formatDate = date => date ? new Date(date).toLocaleString() : "";

export default function ({trips}) {
    const ts = trips.map(t => {
        return (
            <div className="row" key={t.id}>
                <div className="col-sm-1">
                    <Link to={`/client/trips/${t.id}`}>{t.id}</Link>
                </div>
                <div className="col-sm-3">
                    {formatDate(t.startDate)}
                </div>
                <div className="col-sm-3">
                    {formatDate(t.endDate)}
                </div>
            </div>
        );
    });
    return (<div className="container">{ts}</div>);
}

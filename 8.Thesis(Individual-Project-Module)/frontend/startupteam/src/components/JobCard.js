import React from 'react';
import { Link } from 'react-router-dom';

function JobCard({ job }) {
    return (
        <div className="card job-card">
            <div className="card-body">
                <h5 className="card-title">{job.jobTitle}</h5>
                <div className="d-flex align-items-center mb-2">
                    <span className="material-symbols-outlined me-2">business</span>
                    <p className="card-text mb-0"><strong>{job.startupName}</strong></p>
                </div>
                <div className="d-flex align-items-center mb-2">
                    <span className="material-symbols-outlined me-2">location_on</span>
                    <p className="card-text mb-0">{job.jobLocation}</p>
                </div>
                <div className="d-flex align-items-center mb-2">
                    <span className="material-symbols-outlined me-2">calendar_today</span>
                    <p className="card-text mb-0">Deadline: {new Date(job.applicationDeadline).toLocaleDateString()}</p>
                </div>
                <Link to={`/jobs/${job.id}`} className="btn btn-primary">More Details</Link>
            </div>
        </div>
    );
}

export default JobCard;

import React from 'react'
import { Link, Outlet } from 'react-router-dom'
import SignOut from '../Auth/SignOut';
import { useAuth } from '../../context/AuthContext';

function Dashboard() {
    const { authState, hasRole } = useAuth();
    const currentYear = new Date().getFullYear();
    const userName = authState.userfirstName;

    return (
        <>
            <header>
                <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
                    <div className="container-fluid">
                        <img className="navbar-brand" src="/images/logo.svg" alt="StartupTeam Logo" />
                        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                            aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                        </button>
                        <div className="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                            <ul className="navbar-nav flex-grow-1">
                                <li className="nav-item">
                                    <Link className="nav-link text-dark" to="/">Dashboard</Link>
                                </li>

                                {hasRole('StartupFounder') && (
                                    <>
                                        <li className="nav-item dropdown">
                                            <button className="nav-link text-dark dropdown-toggle" id="jobsDropdown" data-bs-toggle="dropdown" aria-expanded="false">
                                                Jobs
                                            </button>
                                            <ul className="dropdown-menu" aria-labelledby="jobsDropdown">
                                                <li>
                                                    <Link className="dropdown-item" to="jobs">Job Listings</Link>
                                                </li>
                                                <li>
                                                    <Link className="dropdown-item" to="jobs/manage">Manage Jobs</Link>
                                                </li>
                                            </ul>
                                        </li>
                                        <li className="nav-item">
                                            <Link className="nav-link text-dark" to="job-applications/manage">Job Applications</Link>
                                        </li>
                                        <li className="nav-item">
                                            <Link className="nav-link text-dark" to="teams/manage">Teams</Link>
                                        </li>
                                    </>
                                )}

                                {hasRole('SkilledIndividual') && (
                                    <>
                                        <li className="nav-item">
                                            <Link className="nav-link text-dark" to="jobs">Job Listings</Link>
                                        </li>
                                        <li className="nav-item">
                                            <Link className="nav-link text-dark" to="job-applications">Job Applications</Link>
                                        </li>
                                        <li className="nav-item">
                                            <Link className="nav-link text-dark" to="teams">Teams</Link>
                                        </li>
                                        <li className="nav-item">
                                            <Link className="nav-link text-dark" to="portfolio/manage">Portfolio</Link>
                                        </li>
                                        <li className="nav-item">
                                            <Link className="nav-link text-dark" to="job-matches">Job Matches</Link>
                                        </li>
                                    </>
                                )}
                                <li className="nav-item">
                                    <Link className="nav-link text-dark" to="users">Users</Link>
                                </li>
                            </ul>
                            <ul className="navbar-nav flex-grow-1 justify-content-end pr-5">
                                <li className="nav-item">
                                    <div className="dropdown mr-5 dropstart">
                                        <button className="btn btn-outline-primary dropdown-toggle" type="button" id="navDropdownMenuButton" data-bs-toggle="dropdown" aria-expanded="false">
                                            Hi {userName}
                                        </button>
                                        <ul className="dropdown-menu" aria-labelledby="navDropdownMenuButton">
                                            <li>
                                                <Link className="dropdown-item" to="profile">
                                                    <span className="material-symbols-outlined me-2">
                                                        person
                                                    </span>
                                                    Profile
                                                </Link>
                                            </li>
                                            <li><hr className="dropdown-divider" /></li>

                                            <SignOut />
                                        </ul>
                                    </div>
                                </li>
                            </ul>
                        </div>
                    </div>
                </nav>
            </header>

            <div className="container">
                {/* Main content or nested routes will go here */}
                <Outlet />
            </div>

            <footer className="border-top footer text-muted">
                <div className="container text-center">
                    &copy; {currentYear} - StartupTeam
                </div>
            </footer>
        </>
    );
}

export default Dashboard;

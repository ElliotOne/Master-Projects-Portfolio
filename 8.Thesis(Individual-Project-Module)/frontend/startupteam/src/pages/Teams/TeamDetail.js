import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { fetchData } from '../../utils/fetchData';
import { handleApiErrors } from '../../utils/handleApiErrors';
import { useAuth } from '../../context/AuthContext';
import SetPageTitle from '../../components/SetPageTitle';
import BackButton from '../../components/BackButton';

function TeamDetail() {
    const { id } = useParams();
    const [team, setTeam] = useState({});
    const [members, setMembers] = useState([]);
    const [roles, setRoles] = useState([]);
    const [goals, setGoals] = useState([]);
    const [milestones, setMilestones] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const { hasRole } = useAuth();

    useEffect(() => {
        const fetchAllData = async () => {
            try {
                // Fetch team details
                const teamResponse = await fetchData(`/teams/details/${id}`);
                handleApiErrors(teamResponse, () => setError('Failed to load team data.'), false, setError);
                setTeam(teamResponse.data);

                // Fetch members
                const membersResponse = await fetchData(`/teams/${id}/members`);
                handleApiErrors(membersResponse, () => setError('Failed to load team members.'), false, setError);
                setMembers(membersResponse.data);

                // Fetch roles
                const rolesResponse = await fetchData(`/teams/${id}/roles`);
                handleApiErrors(rolesResponse, () => setError('Failed to load team roles.'), false, setError);
                setRoles(rolesResponse.data);

                // Fetch goals
                const goalsResponse = await fetchData(`/teams/${id}/goals`);
                handleApiErrors(goalsResponse, () => setError('Failed to load team goals.'), false, setError);
                setGoals(goalsResponse.data);

                // Fetch milestones
                const milestonesResponse = await fetchData(`/teams/${id}/milestones`);
                handleApiErrors(milestonesResponse, () => setError('Failed to load team milestones.'), false, setError);
                setMilestones(milestonesResponse.data);
            } catch (error) {
                setError('An unexpected error occurred.');
                console.error('Failed to fetch team details:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchAllData();
    }, [id]);

    function TruncatedText({ text, limit }) {
        const [isExpanded, setIsExpanded] = useState(false);

        if (text.length <= limit) {
            return <span>{text}</span>;
        }

        return (
            <span>
                {isExpanded ? text : `${text.substring(0, limit)}...`}
                <a
                    href="#"
                    onClick={(e) => {
                        e.preventDefault();
                        setIsExpanded(!isExpanded);
                    }}
                >
                    {isExpanded ? ' Show Less' : ' Read More'}
                </a>
            </span>
        );
    }

    // Format the date to yyyy-MM-dd
    const formatDate = (dateString) => {
        if (!dateString) return '';
        const [datePart] = dateString.split('T');
        return datePart;
    };

    if (loading) {
        return <div className="text-center mt-5">Loading...</div>;
    }

    if (error) {
        return <div className="text-center mt-5">{error}</div>;
    }

    return (
        <section className="team-detail-section mt-4">
            <SetPageTitle title="Team Detail" />
            <div className="container">
                <div className="row mb-5">
                    <div className="col-12">
                        <div className="d-flex justify-content-between align-items-center">
                            <h1 className="mb-4">{team.name}</h1>

                            {hasRole('StartupFounder') && (
                                <Link to={`/teams/manage/edit/${id}`} className="btn btn-secondary">
                                    Edit Team
                                </Link>
                            )}
                        </div>
                    </div>
                    <div className="col-12">
                        <p>{team.description}</p>
                    </div>
                </div>
                <div className="row">
                    <div className="col-12 team-detail-sub-section">
                        <h4>Members</h4>

                        {hasRole('StartupFounder') && (
                            <Link to={`members/new`} className="btn btn-success mb-2">
                                <span className="material-symbols-outlined">add</span>
                                Add Member
                            </Link>
                        )}

                        <table className="table table-striped">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Name</th>
                                    <th>Role</th>
                                    <th></th>
                                    <th></th>
                                    <th className="w-15"></th>
                                </tr>
                            </thead>
                            <tbody>
                                {members.length > 0 ? (
                                    members.map((member, index) => (
                                        <tr key={member.id}>
                                            <td>{index + 1}</td>
                                            <td>{member.individualFullName}</td>
                                            <td>{member.teamRoleName}</td>
                                            <td></td>
                                            <td></td>
                                            <td className="w-15">
                                                {hasRole('StartupFounder') && (
                                                    <Link to={`members/edit/${member.id}`} className="btn btn-sm btn-secondary">
                                                        Edit
                                                    </Link>
                                                )}

                                                <Link to={`/users/${member.individualUserName}`} className="btn btn-sm btn-info ms-2">
                                                    View Profile
                                                </Link>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan="6" className="text-center">No members available.</td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                    <div className="col-12 team-detail-sub-section">
                        <h4>Roles</h4>

                        {hasRole('StartupFounder') && (
                            <Link to={`roles/new`} className="btn btn-success mb-2">
                                <span className="material-symbols-outlined">add</span>
                                Add Role
                            </Link>
                        )}

                        <table className="table table-striped">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Name</th>
                                    <th>Description</th>
                                    <th></th>
                                    <th></th>
                                    <th className="w-15"></th>
                                </tr>
                            </thead>
                            <tbody>
                                {roles.length > 0 ? (
                                    roles.map((role, index) => (
                                        <tr key={role.id}>
                                            <td>{index + 1}</td>
                                            <td>{role.name}</td>
                                            <td>
                                                <TruncatedText text={role.description} limit={50} />
                                            </td>
                                            <td></td>
                                            <td></td>
                                            <td className="w-15">
                                                {hasRole('StartupFounder') && (
                                                    <Link to={`roles/edit/${role.id}`} className="btn btn-sm btn-secondary">
                                                        Edit
                                                    </Link>
                                                )}
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan="6" className="text-center">No roles available.</td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                    <div className="col-12 team-detail-sub-section">
                        <h4>Goals</h4>

                        {hasRole('StartupFounder') && (
                            <Link to={`goals/new`} className="btn btn-success mb-2">
                                <span className="material-symbols-outlined">add</span>
                                Add Goal
                            </Link>
                        )}

                        <table className="table table-striped">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Title</th>
                                    <th>Description</th>
                                    <th>Due Date</th>
                                    <th>Status</th>
                                    <th className="w-15"></th>
                                </tr>
                            </thead>
                            <tbody>
                                {goals.length > 0 ? (
                                    goals.map((goal, index) => (
                                        <tr key={goal.id}>
                                            <td>{index + 1}</td>
                                            <td>{goal.title}</td>
                                            <td>
                                                <TruncatedText text={goal.description} limit={50} />
                                            </td>
                                            <td>{formatDate(goal.dueDate)}</td>
                                            <td>{goal.status}</td>
                                            <td className="w-15">
                                                {hasRole('StartupFounder') && (
                                                    <Link to={`goals/edit/${goal.id}`} className="btn btn-sm btn-secondary">
                                                        Edit
                                                    </Link>
                                                )}
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan="6" className="text-center">No goals available.</td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                    <div className="col-12 team-detail-sub-section">
                        <h4>Milestones</h4>

                        {hasRole('StartupFounder') && (
                            <Link to={`milestones/new`} className="btn btn-success mb-2">
                                <span className="material-symbols-outlined">add</span>
                                Add Milestone
                            </Link>
                        )}

                        <table className="table table-striped">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Title</th>
                                    <th>Description</th>
                                    <th>Due Date</th>
                                    <th>Status</th>
                                    <th className="w-15"></th>
                                </tr>
                            </thead>
                            <tbody>
                                {milestones.length > 0 ? (
                                    milestones.map((milestone, index) => (
                                        <tr key={milestone.id}>
                                            <td>{index + 1}</td>
                                            <td>
                                                {milestone.title}
                                                {milestone.goalTitle && (
                                                    <div className="text-muted small">Goal: {milestone.goalTitle}</div>
                                                )}
                                            </td>
                                            <td>
                                                <TruncatedText text={milestone.description} limit={50} />
                                            </td>
                                            <td>{formatDate(milestone.dueDate)}</td>
                                            <td>{milestone.status}</td>
                                            <td className="w-15">
                                                {hasRole('StartupFounder') && (
                                                    <Link to={`milestones/edit/${milestone.id}`} className="btn btn-sm btn-secondary">
                                                        Edit
                                                    </Link>
                                                )}
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan="6" className="text-center">No milestones available.</td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                    <div className="col-12 text-center mt-5">
                        <BackButton className="btn btn-secondary" />
                    </div>
                </div>
            </div>
        </section>
    );
}

export default TeamDetail;

import React from 'react';
import { BrowserRouter as Router, Route, Routes, Form } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { Dashboard, DashboardOverview } from './pages/Dashboard';
import { SignIn, SignUp, CompleteSignUp, ConfirmEmail, SignUpSuccess, Unauthorized } from './pages/Auth';
import { JobApplicationList, JobApplicationForm, JobApplicationDetail, JobApplicationManagement } from './pages/JobApplications'
import { JobList, JobForm, JobDetail, JobsManagement } from './pages/Jobs';
import { PrivacyPolicy, TermsOfService } from './pages/Legal';
import { ApplicantMatches, JobMatches } from './pages/Matches';
import { PortfolioManagement, PortfolioItemDetail, PortfolioItemForm } from './pages/Portfolios';
import { GoalForm, MemberForm, TeamRoleForm, MilestoneForm, TeamDetail, TeamForm, TeamList, TeamManagement } from './pages/Teams';
import { ProfileForm, UserList, UserProfile } from './pages/Users';
import PrivateRoute from './components/PrivateRoute';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import './App.css';

function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="App">
          <Routes>
            <Route path="/" element={<PrivateRoute element={Dashboard} />}>
              <Route index element={<DashboardOverview />} />

              <Route path="job-applications/:id" element={<JobApplicationDetail />} />
              <Route
                path="job-applications"
                element={<PrivateRoute element={JobApplicationList} requiredRole="SkilledIndividual" />}
              />
              <Route
                path="job-applications/manage"
                element={<PrivateRoute element={JobApplicationManagement} requiredRole="StartupFounder" />}
              />
              <Route
                path="job-applications/manage/edit/:id"
                element={<PrivateRoute element={JobApplicationForm} requiredRole="StartupFounder" />}
              />

              <Route path="jobs" element={<JobList />} />
              <Route path="jobs/:id" element={<JobDetail />} />
              <Route
                path="jobs/manage"
                element={<PrivateRoute element={JobsManagement} requiredRole="StartupFounder" />}
              />
              <Route
                path="jobs/manage/new"
                element={<PrivateRoute element={JobForm} requiredRole="StartupFounder" />}
              />
              <Route
                path="jobs/manage/edit/:id"
                element={<PrivateRoute element={JobForm} requiredRole="StartupFounder" />}
              />

              <Route path="teams" element={<TeamList />} />
              <Route path="teams/:id" element={<TeamDetail />} />

              <Route
                path="teams/manage"
                element={<PrivateRoute element={TeamManagement} requiredRole="StartupFounder" />}
              />
              <Route
                path="teams/manage/new"
                element={<PrivateRoute element={TeamForm} requiredRole="StartupFounder" />}
              />
              <Route
                path="teams/manage/edit/:id"
                element={<PrivateRoute element={TeamForm} requiredRole="StartupFounder" />}
              />

              <Route
                path="teams/:teamId/goals/new"
                element={<PrivateRoute element={GoalForm} requiredRole="StartupFounder" />}
              />
              <Route
                path="teams/:teamId/goals/edit/:id"
                element={<PrivateRoute element={GoalForm} requiredRole="StartupFounder" />}
              />

              <Route
                path="teams/:teamId/members/new"
                element={<PrivateRoute element={MemberForm} requiredRole="StartupFounder" />}
              />
              <Route
                path="teams/:teamId/members/edit/:id"
                element={<PrivateRoute element={MemberForm} requiredRole="StartupFounder" />}
              />

              <Route
                path="teams/:teamId/roles/new"
                element={<PrivateRoute element={TeamRoleForm} requiredRole="StartupFounder" />}
              />
              <Route
                path="teams/:teamId/roles/edit/:id"
                element={<PrivateRoute element={TeamRoleForm} requiredRole="StartupFounder" />}
              />

              <Route
                path="teams/:teamId/milestones/new"
                element={<PrivateRoute element={MilestoneForm} requiredRole="StartupFounder" />}
              />
              <Route
                path="teams/:teamId/milestones/edit/:id"
                element={<PrivateRoute element={MilestoneForm} requiredRole="StartupFounder" />}
              />

              <Route path="portfolio/:id" element={<PortfolioItemDetail />} />
              <Route
                path="portfolio/manage"
                element={<PrivateRoute element={PortfolioManagement} requiredRole="SkilledIndividual" />}
              />
              <Route
                path="portfolio/manage/new"
                element={<PrivateRoute element={PortfolioItemForm} requiredRole="SkilledIndividual" />}
              />
              <Route
                path="portfolio/manage/edit/:id"
                element={<PrivateRoute element={PortfolioItemForm} requiredRole="SkilledIndividual" />}
              />

              <Route
                path="applicant-matches"
                element={<PrivateRoute element={ApplicantMatches} requiredRole="StartupFounder" />}
              />
              <Route
                path="job-matches"
                element={<PrivateRoute element={JobMatches} requiredRole="SkilledIndividual" />}
              />

              <Route path="profile" element={<ProfileForm />} />
              <Route path="users" element={<UserList />} />
              <Route path="users/:id" element={<UserProfile />} />

              <Route path="/unauthorized" element={<Unauthorized />} />
            </Route>

            <Route path="/signin" element={<SignIn />} />
            <Route path="/signup" element={<SignUp />} />
            <Route path="/complete-signup" element={<CompleteSignUp />} />
            <Route path="/confirm-email" element={<ConfirmEmail />} />
            <Route path="/signup-success" element={<SignUpSuccess />} />
            <Route path="/privacy-policy" element={<PrivacyPolicy />} />
            <Route path="/terms-of-service" element={<TermsOfService />} />
          </Routes>
        </div>
      </Router >
    </AuthProvider >
  );
}

export default App;

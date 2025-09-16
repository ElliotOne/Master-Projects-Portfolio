import React from 'react';
import BackButton from '../../components/BackButton';
import SetPageTitle from '../../components/SetPageTitle';

function PrivacyPolicy() {
    return (
        <div className="container">
            <SetPageTitle title="Privacy Policy" />
            <section className="form-section">
                <div className="row justify-content-center">
                    <div className="col-12 mb-5">
                        <div className="section-title text-center">
                            <h3>Privacy Policy</h3>
                        </div>
                    </div>
                    <div className="col-lg-6 col-md-8 col-sm-12 col-12">
                        <p>
                            At StartupTeam, we are committed to protecting your privacy. This Privacy Policy explains how we collect, use, and share your personal information.
                        </p>
                        <h3>1. Information We Collect</h3>
                        <p>
                            We collect personal information that you provide to us directly, such as your name, email address, and any other details you choose to share.
                        </p>
                        <h3>2. How We Use Your Information</h3>
                        <p>
                            We use the information to provide and improve our services, communicate with you, and ensure the security of our platform.
                        </p>
                        <h3>3. Sharing Your Information</h3>
                        <p>
                            We do not share your personal information with third parties without your consent, except as required by law.
                        </p>
                        <h3>4. Use of Local Storage and JWT</h3>
                        <p>
                            We use Local Storage to securely store JSON Web Tokens (JWTs) to manage your authentication session. This allows us to keep you logged in while you use our services. The JWT does not contain any personal information but acts as a reference to your session.
                        </p>
                        <h3>5. Google Authentication</h3>
                        <p>
                            We integrate Google Authentication to provide a convenient login option. Please note that when you log in with Google, Google may set cookies on your device. These cookies are controlled by Google, and we recommend reviewing Google's privacy policy for more information.
                        </p>
                        <h3>6. Your Rights</h3>
                        <p>
                            You have the right to access, correct, or delete your personal information at any time. For any such requests, please contact us at <a href="mailto:Ali.Momenzadeh-Kholenjani@city.ac.uk">Ali.Momenzadeh-Kholenjani@city.ac.uk</a>.
                        </p>
                        <p>Last updated: September 2024</p>
                    </div>
                    <div className="col-12 text-center mt-4">
                        <BackButton />
                    </div>
                </div>
            </section>
        </div>
    );
}

export default PrivacyPolicy;

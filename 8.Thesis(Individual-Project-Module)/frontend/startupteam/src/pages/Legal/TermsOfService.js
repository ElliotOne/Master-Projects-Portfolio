import React from 'react';
import BackButton from '../../components/BackButton';
import SetPageTitle from '../../components/SetPageTitle';

function TermsOfService() {
    return (
        <div className="container">
            <SetPageTitle title="Terms of Service" />
            <section className="form-section">
                <div className="row justify-content-center">
                    <div className="col-12 mb-5">
                        <div className="section-title text-center">
                            <h3>Terms of Service</h3>
                        </div>
                    </div>
                    <div className="col-lg-6 col-md-8 col-sm-12 col-12">
                        <p>
                            Welcome to StartupTeam! These Terms of Service ("Terms") set the guidelines for your use of our website and related services ("Services"). By accessing or using our Services, you agree to follow and be bound by these Terms. Please take a moment to read them thoroughly.
                        </p>
                        <h3>1. Agreement to Terms</h3>
                        <p>
                            When you use our Services, you're agreeing to comply with these Terms, our Privacy Policy, and any other rules that may apply. If you disagree with any part of these Terms, you should not use our Services.
                        </p>
                        <h3>2. Updates to Terms</h3>
                        <p>
                            We may revise these Terms from time to time. Any updates will be effective as soon as they’re posted on our site. By continuing to use our Services after changes are made, you agree to the revised Terms.
                        </p>
                        <h3>3. Your Responsibilities</h3>
                        <p>
                            You are responsible for how you use our Services and for any content you share. You agree not to engage in any illegal or prohibited activities, such as:
                        </p>
                        <ul>
                            <li>Breaking any laws or regulations.</li>
                            <li>Infringing on the intellectual property rights of others.</li>
                            <li>Distributing harmful software or viruses.</li>
                            <li>Engaging in fraudulent or misleading activities.</li>
                        </ul>
                        <h3>4. Creating an Account</h3>
                        <p>
                            Some features of our Services may require you to create an account. You agree to provide accurate and up-to-date information during the registration process and to keep this information current.
                        </p>
                        <p>
                            You are responsible for safeguarding your account credentials and for any actions taken under your account. If you suspect unauthorized use of your account, please notify us immediately.
                        </p>
                        <h3>5. Intellectual Property Rights</h3>
                        <p>
                            The content and materials provided through our Services, including text, graphics, logos, and software, are the property of StartupTeam or our licensors and are protected by intellectual property laws. You may not use, copy, or distribute any content from our Services without our express written permission.
                        </p>
                        <h3>6. Termination of Access</h3>
                        <p>
                            We reserve the right to suspend or terminate your access to our Services at any time, for any reason, and without notice. If your access is terminated, you must stop using the Services immediately.
                        </p>
                        <h3>7. No Warranties</h3>
                        <p>
                            Our Services are provided "as is" and "as available," without any warranties of any kind. We do not guarantee that the Services will be uninterrupted or error-free, or that you will achieve specific results from using them.
                        </p>
                        <h3>8. Limitation of Liability</h3>
                        <p>
                            To the maximum extent permitted by law, StartupTeam and its affiliates, officers, employees, and agents will not be liable for any indirect, incidental, special, consequential, or punitive damages, or for any loss of profits, revenue, data, or goodwill that may result from (i) your use or inability to use the Services; (ii) unauthorized access to or alteration of your data; (iii) any interruption or cessation of the Services; (iv) bugs, viruses, or other harmful elements transmitted through the Services; (v) errors or omissions in any content; or (vi) the actions or statements of any third party using the Services.
                        </p>
                        <h3>9. Governing Law</h3>
                        <p>
                            These Terms are governed by and interpreted in accordance with the laws of England and Wales. Any disputes arising from these Terms or your use of the Services will be subject to the exclusive jurisdiction of the courts of England and Wales.
                        </p>
                        <h3>10. Contact Us</h3>
                        <p>
                            If you have any questions or concerns about these Terms, feel free to reach out to us at <a href="mailto:Ali.Momenzadeh-Kholenjani@city.ac.uk">Ali.Momenzadeh-Kholenjani@city.ac.uk</a>.
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

export default TermsOfService;

import React from 'react';
import SetPageTitle from './SetPageTitle';

const Form = ({ title, onSubmit, children }) => {
    return (
        <section className="form-section">
            <SetPageTitle title={title} />
            <div className="row justify-content-center">
                <div className="col-12 mb-5">
                    <div className="section-title text-center">
                        <h2>{title}</h2>
                    </div>
                </div>
                <div className="col-lg-6 col-md-8 col-sm-12 col-12">
                    <form onSubmit={onSubmit}>
                        {children}
                    </form>
                </div>
            </div>
        </section>
    );
};

export default Form;

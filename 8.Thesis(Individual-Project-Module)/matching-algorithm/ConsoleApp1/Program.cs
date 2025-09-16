using ConsoleApp1.Models;

namespace ConsoleApp1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            double[] thresholds = { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 };

            foreach (var threshold in thresholds)
            {
                Console.WriteLine("==========================================================");
                Console.WriteLine($"========== TESTING WITH THRESHOLD: {threshold:F1} ==========");
                Console.WriteLine("==========================================================\n");

                Console.WriteLine("========== SIMPLE DOCUMENT SIMILARITY MATCHING TEST ==========");
                TestDocumentSimilarityMatching(threshold);
                Console.WriteLine("==============================================================\n");

                Console.WriteLine("========== INDIVIDUAL MATCHING WITH JOB ADS TEST ==========");
                TestIndividualMatchingJobAds(threshold);
                Console.WriteLine("===========================================================\n");

                Console.WriteLine("========== JOB APPLICATIONS MATCHING WITH JOB ADS TEST ==========");
                TestJobApplicationsMatchingJobAds(threshold);
                Console.WriteLine("=================================================================\n");

                Console.WriteLine("************** COMPLEX TEST SCENARIOS BEGIN **************\n");

                Console.WriteLine("========== COMPLEX PORTFOLIO MATCHING WITH JOB ADS TEST ==========");
                TestComplexPortfolioMatchingJobAds(threshold);
                Console.WriteLine("==================================================================\n");

                Console.WriteLine("========== COMPLEX JOB APPLICATIONS MATCHING TEST ==========");
                TestComplexJobApplicationsMatchingJobAds(threshold);
                Console.WriteLine("=============================================================\n");

                Console.WriteLine("************** EDGE CASES BEGIN **************\n");

                Console.WriteLine("========== TESTING IRRELEVANT PORTFOLIO ITEMS ==========");
                TestIrrelevantPortfolioItems(threshold);
                Console.WriteLine("==========================================================\n");

                Console.WriteLine("========== TESTING MISMATCH BETWEEN PORTFOLIO AND CV ITEMS ==========");
                TestPartialPortfolioCvAlignment(threshold);
                Console.WriteLine("==========================================================\n");

                Console.WriteLine("==========================================================");
                Console.WriteLine($"========== END OF TESTS FOR THRESHOLD: {threshold:F1} ==========");
                Console.WriteLine("==========================================================\n\n");
            }
        }


        // Tests document similarity using a collection of example documents. 
        public static void TestDocumentSimilarityMatching(double threshold)
        {
            // Documents
            var documents = new List<string>
            {
                "The quick brown fox jumps over the lazy dog.",
                "Artificial intelligence and machine learning are revolutionizing many industries.",
                "Data science involves analyzing data and building predictive models.",
                "A new smartphone was released today, featuring a high-resolution camera and fast processor.",
                "Machine learning and artificial intelligence are key areas of data science.",
                "The weather today is sunny with a chance of rain in the evening.",
                "The football team won the championship after a long and hard-fought season."
            };

            // Ground truth (expected matches)
            var groundTruth = new List<(int doc1, int doc2, bool isMatch)>
            {
                // Matches (True Positives)
                (2, 5, true),
                (3, 5, true),

                // Non-Matches (True Negatives)
                (1, 6, false),
                (1, 7, false),
                (2, 6, false),
                (2, 7, false),
                (3, 6, false),
                (3, 7, false),
                (4, 6, false),
                (4, 7, false),

                // Possible Matches (not sure but can argue either way)
                (1, 2, false),
                (1, 3, false),
                (1, 4, false),
                (1, 5, false),
                (4, 5, false),
                (6, 7, false),
            };

            var predictions = new List<bool>();
            //var threshold = 0.3; // A threshold for determining a match

            var similarityCalculator = new TextSimilarityCalculator();

            foreach (var (doc1, doc2, _) in groundTruth)
            {
                var score = similarityCalculator.CalculateAverageMatchingScore(documents[doc1 - 1],
                    documents[doc2 - 1]);
                var isMatch = score > threshold;
                predictions.Add(isMatch);
                Console.WriteLine($"Document {doc1} vs Document {doc2}, Score: {score:F6}, Predicted Match: {isMatch}");
            }

            // Extract actual matches from the ground truth for comparison
            var actualMatches = groundTruth.Select(g => g.isMatch).ToList();

            // Calculate metrics based on actual matches vs. predicted matches
            var (accuracy, precision, recall, f1Score) = EvaluationMetrics.CalculateMetrics(actualMatches, predictions);
            Console.WriteLine(
                $"Accuracy: {accuracy:F2}, Precision: {precision:F2}, Recall: {recall:F2}, F1 Score: {f1Score:F2}");

            // Uncomment to print similarity of documents
            //for (int i = 0; i < documents.Count; i++)
            //{
            //    for (int j = i + 1; j < documents.Count; j++)
            //    {
            //        var score = similarityCalculator.CalculateAverageMatchingScore(documents[i], documents[j]);
            //        Console.WriteLine($"Document {i + 1} vs Document {j + 1}, Score: {score:F6}");
            //    }
            //}
        }


        // Tests matching a skilled individual's portfolio with available job advertisements.
        public static void TestIndividualMatchingJobAds(double threshold)
        {
            var jobAds = new List<JobAdvertisement>
            {
                new JobAdvertisement
                {
                    JobTitle = "Data Scientist",
                    JobDescription = "Analyze data and build machine learning models.",
                    RequiredSkills = "Python,Machine Learning,Data Analysis",
                    KeyTechnologies = "TensorFlow,Pandas,Python",
                    Industry = "Tech",
                    Experience = "3+ years in Data Science",
                    Education = "Bachelor's in Computer Science",
                    StartupDescription = "Tech startup focused on AI solutions."
                },
                new JobAdvertisement
                {
                    JobTitle = "Software Engineer",
                    JobDescription = "Develop software using C# and .NET.",
                    RequiredSkills = "C#,ASP.NET,SQL",
                    KeyTechnologies = "Azure,.NET,C#",
                    Industry = "Finance",
                    Experience = "2+ years in Software Development",
                    Education = "Bachelor's in Software Engineering",
                    StartupDescription = "FinTech company developing financial solutions."
                },
                new JobAdvertisement
                {
                    JobTitle = "Cloud Engineer",
                    JobDescription = "Design and manage cloud infrastructure for scalable applications.",
                    RequiredSkills = "AWS,Azure,Cloud Computing",
                    KeyTechnologies = "AWS,Azure,Kubernetes",
                    Industry = "Tech",
                    Experience = "3+ years in Cloud Engineering",
                    Education = "Bachelor's in Computer Engineering",
                    StartupDescription = "Startup providing cloud-based solutions."
                }
            };

            var portfolio = new Portfolio
            {
                PortfolioItems = new List<PortfolioItem>
                {
                    new PortfolioItem
                    {
                        Title = "AI Project",
                        Description = "Built a machine learning model for predictive analysis.",
                        Skills = "Python,Machine Learning,Data Analysis",
                        Technologies = "TensorFlow,Python",
                        Industry = "Tech"
                    },
                    new PortfolioItem
                    {
                        Title = "Web App Development",
                        Description = "Developed a web application using ASP.NET.",
                        Skills = "C#,ASP.NET,SQL",
                        Technologies = ".NET,C#",
                        Industry = "Finance"
                    }
                }
            };

            var groundTruthTest = new List<(int portfolioId, int jobAdId, bool isMatch)>
            {
                // Matches (PortfolioItem should match JobAd)
                (0, 0, true), // PortfolioItem 1 should match JobAd 1 (Data Scientist)
                (1, 1, true), // PortfolioItem 2 should match JobAd 2 (Software Engineer)

                // Non-Matches (PortfolioItem should not match JobAd)
                (0, 1, false), // PortfolioItem 1 shouldn't match JobAd 2
                (1, 0, false), // PortfolioItem 2 shouldn't match JobAd 1
                (0, 2, false), // PortfolioItem 1 shouldn't match JobAd 3 (Cloud Engineer)
                (1, 2, false) // PortfolioItem 2 shouldn't match JobAd 3 (Cloud Engineer)
            };

            //var threshold = 0.3;
            var predictions = new List<bool>();

            var similarityCalculator = new TextSimilarityCalculator();

            // Match Portfolio with Job Ads
            foreach (var (portfolioId, jobAdId, _) in groundTruthTest)
            {
                // Using less properties will result in less contextual information thus less accuracy
                //var portfolioText = string.Join(" ",
                //    portfolio.PortfolioItems.Select(item => $"{item.Skills} {item.Technologies} {item.Industry}"));

                //var jobAdText = $"{jobAds[jobAdId].RequiredSkills} {jobAds[jobAdId].KeyTechnologies} {jobAds[jobAdId].Industry} {jobAds[jobAdId].JobDescription}";

                var portfolioItem = portfolio.PortfolioItems.ElementAt(portfolioId);
                var portfolioText =
                    $"{portfolioItem.Title} {portfolioItem.Description} {portfolioItem.Skills} {portfolioItem.Technologies} {portfolioItem.Industry}";

                // Concatenate relevant properties of the job advertisement
                var jobAd = jobAds[jobAdId];
                var jobAdText =
                    $"{jobAd.JobTitle} {jobAd.JobDescription} {jobAd.RequiredSkills} {jobAd.KeyTechnologies} {jobAd.Industry} {jobAd.Experience} {jobAd.Education} {jobAd.StartupDescription}";

                var score = similarityCalculator.CalculateAverageMatchingScore(portfolioText, jobAdText);
                var isMatch = score > threshold;
                predictions.Add(isMatch);

                Console.WriteLine(
                    $"Portfolio {portfolioId} vs JobAd {jobAdId}, Score: {score:F6}, Predicted Match: {isMatch}");
            }

            // Extract actual matches from ground truth for comparison
            var actualMatches = groundTruthTest.Select(g => g.isMatch).ToList();

            // Calculate metrics
            var (accuracy, precision, recall, f1Score) = EvaluationMetrics.CalculateMetrics(actualMatches, predictions);
            Console.WriteLine(
                $"Accuracy: {accuracy:F2}, Precision: {precision:F2}, Recall: {recall:F2}, F1 Score: {f1Score:F2}");
        }


        // Tests matching job applications (from skilled individuals) with job advertisements for startup founders
        public static void TestJobApplicationsMatchingJobAds(double threshold)
        {
            var jobAds = new List<JobAdvertisement>
            {
                new JobAdvertisement
                {
                    JobTitle = "Data Scientist",
                    JobDescription = "Analyze data and build machine learning models.",
                    RequiredSkills = "Python,Machine Learning,Data Analysis",
                    KeyTechnologies = "TensorFlow,Pandas,Python",
                    Industry = "Tech",
                    Experience = "3+ years in Data Science",
                    Education = "Bachelor's in Computer Science",
                    StartupDescription = "Tech startup focused on AI solutions."
                },
                new JobAdvertisement
                {
                    JobTitle = "Software Engineer",
                    JobDescription = "Develop software using C# and .NET.",
                    RequiredSkills = "C#,ASP.NET,SQL",
                    KeyTechnologies = "Azure,.NET,C#",
                    Industry = "Finance",
                    Experience = "2+ years in Software Development",
                    Education = "Bachelor's in Software Engineering",
                    StartupDescription = "FinTech company developing financial solutions."
                }
            };

            var jobApplications = new List<JobApplication>
            {
                new JobApplication
                {
                    JobAdvertisementId = jobAds[0].Id,
                    CVTextContent = "Python Machine Learning Data Analysis TensorFlow",
                },
                new JobApplication
                {
                    JobAdvertisementId = jobAds[1].Id,
                    CVTextContent = "C# ASP.NET SQL .NET",
                }
            };

            var portfolios = new List<Portfolio>
            {
                new Portfolio
                {
                    PortfolioItems = new List<PortfolioItem>
                    {
                        new PortfolioItem
                        {
                            Title = "AI Project",
                            Description = "Built a machine learning model for predictive analysis.",
                            Skills = "Python,Machine Learning,Data Analysis",
                            Technologies = "TensorFlow,Python",
                            Industry = "Tech"
                        }
                    }
                },
                new Portfolio
                {
                    PortfolioItems = new List<PortfolioItem>
                    {
                        new PortfolioItem
                        {
                            Title = "Web App Development",
                            Description = "Developed a web application using ASP.NET.",
                            Skills = "C#,ASP.NET,SQL",
                            Technologies = ".NET,C#",
                            Industry = "Finance"
                        }
                    }
                }
            };

            var groundTruthTest = new List<(int jobAppId, int jobAdId, bool isMatch)>
            {
                // Matches
                (0, 0, true), // JobApplication 1 should match JobAd 1 (Data Scientist)
                (1, 1, true), // JobApplication 2 should match JobAd 2 (Software Engineer)

                // Non-Matches
                (0, 1, false), // JobApplication 1 shouldn't match JobAd 2
                (1, 0, false) // JobApplication 2 shouldn't match JobAd 1
            };

            var predictions = new List<bool>();
            //var threshold = 0.3;

            var similarityCalculator = new TextSimilarityCalculator();

            // Match Job Applications with Job Ads, considering the applicant's portfolio
            foreach (var (jobAppId, jobAdId, _) in groundTruthTest)
            {
                var jobApp = jobApplications[jobAppId];
                var portfolio = portfolios[jobAppId];

                // Concatenate CV text and portfolio properties
                var portfolioText = string.Join(" ",
                    portfolio.PortfolioItems.Select(portfolioItem =>
                        $"{portfolioItem.Title} {portfolioItem.Description} {portfolioItem.Skills} {portfolioItem.Technologies} {portfolioItem.Industry}"));
                var jobAppText = $"{jobApp.CVTextContent} {portfolioText}";

                // Concatenate relevant properties of the job advertisement
                var jobAd = jobAds[jobAdId];
                var jobAdText =
                    $"{jobAd.JobTitle} {jobAd.JobDescription} {jobAd.RequiredSkills} {jobAd.KeyTechnologies} {jobAd.Industry} {jobAd.Experience} {jobAd.Education} {jobAd.StartupDescription}";

                var score = similarityCalculator.CalculateAverageMatchingScore(jobAppText, jobAdText);
                var isMatch = score > threshold;
                predictions.Add(isMatch);

                Console.WriteLine(
                    $"JobApplication {jobAppId} vs JobAd {jobAdId}, Score: {score:F6}, Predicted Match: {isMatch}");
            }

            var actualMatches = groundTruthTest.Select(g => g.isMatch).ToList();
            var (accuracy, precision, recall, f1Score) = EvaluationMetrics.CalculateMetrics(actualMatches, predictions);
            Console.WriteLine(
                $"Accuracy: {accuracy:F2}, Precision: {precision:F2}, Recall: {recall:F2}, F1 Score: {f1Score:F2}");
        }

        // Tests matching a skilled individual's portfolio with more complex job advertisements.
        public static void TestComplexPortfolioMatchingJobAds(double threshold)
        {
            var jobAds = new List<JobAdvertisement>
            {
                new JobAdvertisement
                {
                    JobTitle = "Lead Data Scientist",
                    JobDescription =
                        "Lead a team of data scientists to develop predictive analytics and machine learning models for real-time financial data.",
                    RequiredSkills = "Python,Machine Learning,Deep Learning,Data Analysis,Leadership",
                    KeyTechnologies = "TensorFlow,Pandas,Python,Docker,Kubernetes",
                    Industry = "Finance",
                    Experience = "8+ years in Data Science",
                    Education = "Master's or PhD in Computer Science or related field",
                    StartupDescription = "A FinTech startup revolutionizing financial data analysis using AI."
                },
                new JobAdvertisement
                {
                    JobTitle = "Senior Full Stack Developer",
                    JobDescription =
                        "Develop scalable web applications using modern web frameworks and cloud-native infrastructure.",
                    RequiredSkills = "React.js,Node.js,AWS,Docker,Microservices",
                    KeyTechnologies = "AWS,Lambda,React.js,Node.js,Docker,Kubernetes",
                    Industry = "E-commerce",
                    Experience = "5+ years in Full Stack Development",
                    Education = "Bachelor's in Software Engineering",
                    StartupDescription = "A fast-growing e-commerce platform focused on a seamless shopping experience."
                }
            };

            var portfolio = new Portfolio
            {
                PortfolioItems = new List<PortfolioItem>
                {
                    new PortfolioItem
                    {
                        Title = "Financial Data Prediction",
                        Description =
                            "Led a team to build a real-time financial forecasting model using deep learning techniques.",
                        Skills = "Python,Deep Learning,Data Analysis,Leadership",
                        Technologies = "TensorFlow,Pandas,Python,Docker",
                        Industry = "Finance"
                    },
                    new PortfolioItem
                    {
                        Title = "E-commerce Web Application",
                        Description =
                            "Developed a scalable e-commerce web application using React.js and Node.js, deployed on AWS with a microservices architecture.",
                        Skills = "React.js,Node.js,AWS,Docker,Microservices",
                        Technologies = "AWS,Lambda,React.js,Node.js,Docker,Kubernetes",
                        Industry = "E-commerce"
                    }
                }
            };

            var groundTruthTest = new List<(int portfolioId, int jobAdId, bool isMatch)>
            {
                // Matches
                (0, 0, true), // PortfolioItem 1 should match JobAd 1 (Lead Data Scientist)
                (1, 1, true), // PortfolioItem 2 should match JobAd 2 (Senior Full Stack Developer)

                // Non-Matches
                (0, 1, false), // PortfolioItem 1 shouldn't match JobAd 2
                (1, 0, false) // PortfolioItem 2 shouldn't match JobAd 1
            };

            //var threshold = 0.3;
            var predictions = new List<bool>();

            var similarityCalculator = new TextSimilarityCalculator();

            // Match Portfolio with Job Ads
            foreach (var (portfolioId, jobAdId, _) in groundTruthTest)
            {
                var portfolioItem = portfolio.PortfolioItems.ElementAt(portfolioId);
                var portfolioText =
                    $"{portfolioItem.Title} {portfolioItem.Description} {portfolioItem.Skills} {portfolioItem.Technologies} {portfolioItem.Industry}";

                var jobAd = jobAds[jobAdId];
                var jobAdText =
                    $"{jobAd.JobTitle} {jobAd.JobDescription} {jobAd.RequiredSkills} {jobAd.KeyTechnologies} {jobAd.Industry} {jobAd.Experience} {jobAd.Education} {jobAd.StartupDescription}";

                var score = similarityCalculator.CalculateAverageMatchingScore(portfolioText, jobAdText);
                var isMatch = score > threshold;
                predictions.Add(isMatch);

                Console.WriteLine(
                    $"Portfolio {portfolioId} vs JobAd {jobAdId}, Score: {score:F6}, Predicted Match: {isMatch}");
            }

            var actualMatches = groundTruthTest.Select(g => g.isMatch).ToList();
            var (accuracy, precision, recall, f1Score) = EvaluationMetrics.CalculateMetrics(actualMatches, predictions);
            Console.WriteLine(
                $"Accuracy: {accuracy:F2}, Precision: {precision:F2}, Recall: {recall:F2}, F1 Score: {f1Score:F2}");
        }

        // Tests matching job applications (from skilled individuals) with more complex job advertisements for startup founders
        public static void TestComplexJobApplicationsMatchingJobAds(double threshold)
        {
            var jobAds = new List<JobAdvertisement>
            {
                new JobAdvertisement
                {
                    JobTitle = "Cybersecurity Engineer",
                    JobDescription =
                        "Design and implement secure network architectures, perform security audits, and develop incident response plans.",
                    RequiredSkills = "Cybersecurity,Network Security,Penetration Testing,Firewalls,Encryption",
                    KeyTechnologies = "AWS,Azure,Cloud Security,Kubernetes,Encryption",
                    Industry = "Tech",
                    Experience = "5+ years in Cybersecurity",
                    Education = "Bachelor's or higher in Information Security or related field",
                    StartupDescription =
                        "A tech startup focusing on providing end-to-end cybersecurity solutions for cloud-based platforms."
                },
                new JobAdvertisement
                {
                    JobTitle = "AI Research Scientist",
                    JobDescription =
                        "Research and develop novel AI algorithms for natural language processing, machine learning, and computer vision.",
                    RequiredSkills = "Machine Learning,Deep Learning,NLP,Computer Vision,AI Research",
                    KeyTechnologies = "TensorFlow,PyTorch,Python,NLP,Computer Vision",
                    Industry = "Healthcare",
                    Experience = "PhD in AI or related field",
                    Education = "PhD in AI, Machine Learning, or related fields",
                    StartupDescription = "A healthcare startup that leverages AI to improve patient outcomes."
                }
            };

            var jobApplications = new List<JobApplication>
            {
                new JobApplication
                {
                    JobAdvertisementId = jobAds[0].Id,
                    CVTextContent =
                        "Cybersecurity Network Security Firewalls Penetration Testing Encryption AWS Cloud Security",
                },
                new JobApplication
                {
                    JobAdvertisementId = jobAds[1].Id,
                    CVTextContent = "Machine Learning Deep Learning NLP Computer Vision TensorFlow PyTorch Python",
                }
            };

            var portfolios = new List<Portfolio>
            {
                new Portfolio
                {
                    PortfolioItems = new List<PortfolioItem>
                    {
                        new PortfolioItem
                        {
                            Title = "Cloud Security Audit",
                            Description =
                                "Conducted a full security audit for a cloud-based platform, identifying vulnerabilities and proposing solutions.",
                            Skills = "Cybersecurity,Cloud Security,Penetration Testing,Encryption",
                            Technologies = "AWS,Azure,Firewalls,Kubernetes",
                            Industry = "Tech"
                        }
                    }
                },
                new Portfolio
                {
                    PortfolioItems = new List<PortfolioItem>
                    {
                        new PortfolioItem
                        {
                            Title = "NLP Model for Healthcare",
                            Description =
                                "Developed a natural language processing model to assist in healthcare decision-making using deep learning.",
                            Skills = "NLP,Deep Learning,Machine Learning,Python",
                            Technologies = "TensorFlow,PyTorch,Python,Computer Vision",
                            Industry = "Healthcare"
                        }
                    }
                }
            };

            var groundTruthTest = new List<(int jobAppId, int jobAdId, bool isMatch)>
            {
                // Matches
                (0, 0, true), // JobApplication 1 should match JobAd 1 (Cybersecurity Engineer)
                (1, 1, true), // JobApplication 2 should match JobAd 2 (AI Research Scientist)

                // Non-Matches
                (0, 1, false), // JobApplication 1 shouldn't match JobAd 2
                (1, 0, false) // JobApplication 2 shouldn't match JobAd 1
            };

            var predictions = new List<bool>();
            //var threshold = 0.3;

            var similarityCalculator = new TextSimilarityCalculator();

            // Match Job Applications with Job Ads, considering the applicant's portfolio
            foreach (var (jobAppId, jobAdId, _) in groundTruthTest)
            {
                var jobApp = jobApplications[jobAppId];
                var portfolio = portfolios[jobAppId];

                var portfolioText = string.Join(" ",
                    portfolio.PortfolioItems.Select(portfolioItem =>
                        $"{portfolioItem.Title} {portfolioItem.Description} {portfolioItem.Skills} {portfolioItem.Technologies} {portfolioItem.Industry}"));
                var jobAppText = $"{jobApp.CVTextContent} {portfolioText}";

                var jobAd = jobAds[jobAdId];
                var jobAdText =
                    $"{jobAd.JobTitle} {jobAd.JobDescription} {jobAd.RequiredSkills} {jobAd.KeyTechnologies} {jobAd.Industry} {jobAd.Experience} {jobAd.Education} {jobAd.StartupDescription}";

                var score = similarityCalculator.CalculateAverageMatchingScore(jobAppText, jobAdText);
                var isMatch = score > threshold;
                predictions.Add(isMatch);

                Console.WriteLine(
                    $"JobApplication {jobAppId} vs JobAd {jobAdId}, Score: {score:F6}, Predicted Match: {isMatch}");

            }

            var actualMatches = groundTruthTest.Select(g => g.isMatch).ToList();
            var (accuracy, precision, recall, f1Score) = EvaluationMetrics.CalculateMetrics(actualMatches, predictions);
            Console.WriteLine(
                $"Accuracy: {accuracy:F2}, Precision: {precision:F2}, Recall: {recall:F2}, F1 Score: {f1Score:F2}");
        }

        // Edge case: Test when portfolio items are completely irrelevant to the job advertisement.
        public static void TestIrrelevantPortfolioItems(double threshold)
        {
            var jobAds = new List<JobAdvertisement>
            {
                new JobAdvertisement
                {
                    JobTitle = "Data Scientist",
                    JobDescription = "Analyze data and build machine learning models.",
                    RequiredSkills = "Python,Machine Learning,Data Analysis",
                    KeyTechnologies = "TensorFlow,Pandas,Python",
                    Industry = "Tech",
                    Experience = "3+ years in Data Science",
                    Education = "Bachelor's in Computer Science",
                    StartupDescription = "Tech startup focused on AI solutions."
                }
            };

            var portfolio = new Portfolio
            {
                PortfolioItems = new List<PortfolioItem>
                {
                    new PortfolioItem
                    {
                        Title = "Graphic Design Project",
                        Description = "Designed marketing materials for a client.",
                        Skills = "Photoshop,Illustrator",
                        Technologies = "Adobe Suite",
                        Industry = "Design"
                    },
                    new PortfolioItem
                    {
                        Title = "Content Writing",
                        Description = "Wrote articles for various clients.",
                        Skills = "Writing,SEO",
                        Technologies = "WordPress",
                        Industry = "Media"
                    }
                }
            };

            var groundTruthTest = new List<(int portfolioId, int jobAdId, bool isMatch)>
            {
                (0, 0, false), // PortfolioItem 1 shouldn't match JobAd (irrelevant skills and industry)
                (1, 0, false)  // PortfolioItem 2 shouldn't match JobAd (irrelevant skills and industry)
            };

            var predictions = new List<bool>();
            var similarityCalculator = new TextSimilarityCalculator();

            foreach (var (portfolioId, jobAdId, _) in groundTruthTest)
            {
                var portfolioItem = portfolio.PortfolioItems.ElementAt(portfolioId);
                var portfolioText = $"{portfolioItem.Title} {portfolioItem.Description} {portfolioItem.Skills} {portfolioItem.Technologies} {portfolioItem.Industry}";

                var jobAd = jobAds[jobAdId];
                var jobAdText = $"{jobAd.JobTitle} {jobAd.JobDescription} {jobAd.RequiredSkills} {jobAd.KeyTechnologies} {jobAd.Industry} {jobAd.Experience} {jobAd.Education} {jobAd.StartupDescription}";

                var score = similarityCalculator.CalculateAverageMatchingScore(portfolioText, jobAdText);
                var isMatch = score > threshold;
                predictions.Add(isMatch);

                Console.WriteLine($"Portfolio {portfolioId} vs JobAd {jobAdId}, Score: {score:F6}, Predicted Match: {isMatch}");
            }

            var actualMatches = groundTruthTest.Select(g => g.isMatch).ToList();
            var (accuracy, precision, recall, f1Score) = EvaluationMetrics.CalculateMetrics(actualMatches, predictions);
            Console.WriteLine($"Accuracy: {accuracy:F2}, Precision: {precision:F2}, Recall: {recall:F2}, F1 Score: {f1Score:F2}");
        }

        // Edge case: Test where the CV aligns with the job ad, but the portfolio item doesn't, and vice versa
        public static void TestPartialPortfolioCvAlignment(double threshold)
        {
            var jobAds = new List<JobAdvertisement>
            {
                new JobAdvertisement
                {
                    JobTitle = "Software Engineer",
                    JobDescription = "Develop software using C# and .NET.",
                    RequiredSkills = "C#,ASP.NET,SQL",
                    KeyTechnologies = "Azure,.NET,C#",
                    Industry = "Finance",
                    Experience = "2+ years in Software Development",
                    Education = "Bachelor's in Software Engineering",
                    StartupDescription = "FinTech company developing financial solutions."
                }
            };

            var jobApplications = new List<JobApplication>
            {
                new JobApplication
                {
                    JobAdvertisementId = jobAds[0].Id,
                    // The CV aligns with the job advertisement (good match)
                    CVTextContent = "C# ASP.NET SQL .NET",
                }
            };

            // Portfolio item is unrelated to the job ad (mismatch)
            var portfolios = new List<Portfolio>
            {
                new Portfolio
                {
                    PortfolioItems = new List<PortfolioItem>
                    {
                        new PortfolioItem
                        {
                            Title = "Photography Project",
                            Description = "Captured high-quality images for a travel blog.",
                            Skills = "Photography,Photo Editing",
                            Technologies = "Adobe Photoshop,DSLR Camera",
                            Industry = "Media"
                        }
                    }
                }
            };

            var groundTruthTest = new List<(int jobAppId, int jobAdId, bool isMatch)>
            {
                // The CV should match the job ad, but the portfolio should not
                (0, 0, false) // The mismatch should lead to no match, so isMatch should be false
            };

            var predictions = new List<bool>();
            var similarityCalculator = new TextSimilarityCalculator();

            // Match Job Application with Job Ad, considering the applicant's portfolio
            foreach (var (jobAppId, jobAdId, _) in groundTruthTest)
            {
                var jobApp = jobApplications[jobAppId];
                var portfolio = portfolios[jobAppId];

                // Concatenate CV text and portfolio properties
                var portfolioText = string.Join(" ",
                    portfolio.PortfolioItems.Select(portfolioItem =>
                        $"{portfolioItem.Title} {portfolioItem.Description} {portfolioItem.Skills} {portfolioItem.Technologies} {portfolioItem.Industry}"));
                var jobAppText = $"{jobApp.CVTextContent} {portfolioText}";

                // Concatenate relevant properties of the job advertisement
                var jobAd = jobAds[jobAdId];
                var jobAdText =
                    $"{jobAd.JobTitle} {jobAd.JobDescription} {jobAd.RequiredSkills} {jobAd.KeyTechnologies} {jobAd.Industry} {jobAd.Experience} {jobAd.Education} {jobAd.StartupDescription}";

                var score = similarityCalculator.CalculateAverageMatchingScore(jobAppText, jobAdText);
                var isMatch = score > threshold;
                predictions.Add(isMatch);

                Console.WriteLine($"JobApplication {jobAppId} vs JobAd {jobAdId}, Score: {score:F6}, Predicted Match: {isMatch}");
            }

            var actualMatches = groundTruthTest.Select(g => g.isMatch).ToList();
            var (accuracy, precision, recall, f1Score) = EvaluationMetrics.CalculateMetrics(actualMatches, predictions);
            Console.WriteLine($"Accuracy: {accuracy:F2}, Precision: {precision:F2}, Recall: {recall:F2}, F1 Score: {f1Score:F2}");
        }
    }
}

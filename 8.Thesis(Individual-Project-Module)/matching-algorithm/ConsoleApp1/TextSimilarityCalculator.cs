using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    /// <summary>
    /// This class provides methods for calculating text similarity between two documents 
    /// using TF-IDF vectorization and Cosine Similarity. It also includes predefined phrases 
    /// to improve relevance for certain industries such as data science, software engineering, 
    /// cloud technologies, and cybersecurity.
    /// </summary>
    public class TextSimilarityCalculator
    {
        private readonly Dictionary<string, int> _documentFrequency = new Dictionary<string, int>();
        private readonly Dictionary<string, double> _idfCache = new Dictionary<string, double>();
        private int _totalDocuments = 0;

        private static readonly HashSet<string> PhraseDictionary = new HashSet<string>
        {
            // Common phrases for data science and machine learning roles
            "machine learning", "deep learning", "artificial intelligence", "data science",
            "data analysis", "predictive analytics", "natural language processing", "computer vision",
            "reinforcement learning", "neural networks", "big data", "data visualization",
            "data engineering", "time series forecasting", "random forest", "support vector machines",

            // Common phrases for software engineering roles
            "web development", "full stack development", "backend development", "frontend development",
            "API design", "RESTful services", "microservices architecture", "cloud computing",
            "agile development", "test-driven development", "continuous integration", "devops",
            "unit testing", "containerization", "docker", "kubernetes", "distributed systems",
            "version control", "git",

            // Common phrases for cloud technologies
            "aws", "azure", "google cloud platform", "serverless architecture",
            "infrastructure as code", "cloud migration",

            // Common phrases for cybersecurity roles
            "cybersecurity", "network security", "penetration testing", "vulnerability assessment",
            "threat intelligence", "incident response", "firewalls", "encryption", "identity management",

            // General tech phrases
            "agile methodology", "project management", "scrum", "jira", "trello",
            "software design patterns", "object-oriented programming", "system design",
            "scalability", "high availability", "distributed systems",

            // Programming languages
            "python", "java", "c#", "javascript", "typescript", "go", "ruby", "php",
            "rust", "scala", "swift", "kotlin",

            // Databases and storage technologies
            "sql", "nosql", "mongodb", "postgresql", "mysql", "redis", "elasticsearch",
            "data lakes", "data warehousing",

            // Additional tech tools
            "gitlab", "github", "jenkins", "circleci", "terraform", "ansible", "puppet"
        };

        // Define a list of common stop words
        private static readonly HashSet<string> StopWords = new HashSet<string>
        {
            "a", "an", "the", "and", "is", "in", "at", "on", "of", "for", "with", "to",
            "from", "by", "it", "this", "that", // add more as needed
        };

        // Method to calculate average matching score using both unigrams (N = 1) and bigrams (N = 2)
        public double CalculateAverageMatchingScore(string docA, string docB)
        {
            // Calculate unigrams matching score (N = 1)
            var unigramScore = CalculateMatchingScore(docA, docB, 1);

            // Calculate bigrams matching score (N = 2)
            var bigramScore = CalculateMatchingScore(docA, docB, 2);

            // Return the average of unigram and bigram scores
            return (unigramScore + bigramScore) / 2.0;
        }

        // Method to calculate matching score between two documents using specific N-Grams
        public double CalculateMatchingScore(string docA, string docB, int n)
        {
            var vectorA = Transform(new[] { docA }, n);
            var vectorB = Transform(new[] { docB }, n);

            return CosineSimilarity(vectorA, vectorB);
        }

        // TF-IDF Vectorizer that accepts N-Grams
        public Dictionary<string, double> Transform(IEnumerable<string> documents, int n)
        {
            var termFrequency = new Dictionary<string, double>();
            _totalDocuments = documents.Count();

            // Calculate document frequency (DF)
            foreach (var doc in documents)
            {
                var tf = CalculateTermFrequency(doc, n);
                foreach (var term in tf.Keys)
                {
                    if (!_documentFrequency.ContainsKey(term))
                        _documentFrequency[term] = 0;

                    _documentFrequency[term]++;
                }
            }

            // Calculate TF-IDF
            foreach (var doc in documents)
            {
                var tfidf = CalculateTermFrequency(doc, n).ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value * CalculateIDF(kvp.Key)
                );

                foreach (var term in tfidf.Keys)
                {
                    if (!termFrequency.ContainsKey(term))
                        termFrequency[term] = 0;

                    termFrequency[term] += tfidf[term];
                }
            }

            return termFrequency;
        }

        // Calculate Term Frequency (TF) for a specific N-Gram (n)
        private Dictionary<string, double> CalculateTermFrequency(string document, int n)
        {
            var termFrequency = new Dictionary<string, double>();
            var phrases = ExtractPhrases(document, n);

            foreach (var phrase in phrases)
            {
                if (!termFrequency.ContainsKey(phrase))
                    termFrequency[phrase] = 0;

                termFrequency[phrase]++;
            }

            // Normalize by the number of terms
            var totalTerms = termFrequency.Values.Sum();
            foreach (var term in termFrequency.Keys.ToList())
            {
                termFrequency[term] /= totalTerms;
            }

            return termFrequency;
        }

        // Calculate Inverse Document Frequency (IDF)
        private double CalculateIDF(string term)
        {
            if (!_idfCache.ContainsKey(term))
            {
                var df = _documentFrequency.ContainsKey(term) ? _documentFrequency[term] : 0;
                var idf = Math.Log((double)_totalDocuments / (df + 1)) + 1;
                _idfCache[term] = idf;
            }

            return _idfCache[term];
        }

        // Tokenize and extract N-Grams from text
        private IEnumerable<string> Tokenize(string text, int n = 1)
        {
            var words = Regex.Split(text.ToLower(), @"\W+")
                .Where(term => !string.IsNullOrEmpty(term) && !StopWords.Contains(term)) // Exclude stop words
                             .ToList();

            var ngrams = new List<string>();

            for (int i = 0; i < words.Count - n + 1; i++)
            {
                ngrams.Add(string.Join(" ", words.Skip(i).Take(n)));
            }

            return ngrams;
        }

        // Extract predefined phrases and N-Grams from text
        private IEnumerable<string> ExtractPhrases(string text, int n)
        {
            var phrases = new List<string>();
            var lowerText = text.ToLower();

            // Extract predefined phrases
            foreach (var phrase in PhraseDictionary)
            {
                if (lowerText.Contains(phrase))
                {
                    phrases.Add(phrase);
                }
            }

            // Add N-Grams based on the specified value of N
            phrases.AddRange(Tokenize(text, n));

            return phrases;
        }

        // Calculate Cosine Similarity between two vectors
        private double CosineSimilarity(Dictionary<string, double> vectorA, Dictionary<string, double> vectorB)
        {
            var dotProduct = vectorA.Keys.Intersect(vectorB.Keys).Sum(key => vectorA[key] * vectorB[key]);
            var magnitudeA = Math.Sqrt(vectorA.Values.Sum(val => val * val));
            var magnitudeB = Math.Sqrt(vectorB.Values.Sum(val => val * val));

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;

            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}

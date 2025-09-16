namespace StartupTeam.Module.MatchingManagement.Utilities
{
    /// <summary>
    /// This class provides methods to calculate common evaluation metrics, such as 
    /// Accuracy, Precision, Recall, and F1-Score, for classification models. 
    /// It compares the predicted results with the ground truth to compute these metrics.
    /// </summary>
    public static class EvaluationMetrics
    {
        public static (double Accuracy, double Precision, double Recall, double F1Score) CalculateMetrics(
            List<bool> groundTruth, List<bool> predictions)
        {
            int truePositive = 0, trueNegative = 0, falsePositive = 0, falseNegative = 0;

            for (int i = 0; i < groundTruth.Count; i++)
            {
                if (groundTruth[i] && predictions[i])
                    truePositive++;
                else if (!groundTruth[i] && !predictions[i])
                    trueNegative++;
                else if (!groundTruth[i] && predictions[i])
                    falsePositive++;
                else if (groundTruth[i] && !predictions[i])
                    falseNegative++;
            }

            double accuracy = (truePositive + trueNegative) / (double)(truePositive + trueNegative + falsePositive + falseNegative);
            double precision = (truePositive + falsePositive) == 0 ? 0 : truePositive / (double)(truePositive + falsePositive);
            double recall = (truePositive + falseNegative) == 0 ? 0 : truePositive / (double)(truePositive + falseNegative);
            double f1Score = (precision + recall) == 0 ? 0 : 2 * (precision * recall) / (precision + recall);

            return (accuracy, precision, recall, f1Score);
        }
    }
}

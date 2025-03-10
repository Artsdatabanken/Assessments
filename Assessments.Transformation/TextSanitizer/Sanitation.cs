using System;
using System.Collections.Generic;
using System.Linq;
namespace Assessments.Transformation.TextSanitizer
{
    internal class Sanitation
    {
        internal static IEnumerable<dynamic> Sanitize(IEnumerable<dynamic> recordsEnumerable, List<string> targettingFields, string replaceWith, out bool anyThingReplaced)
        {
            var hitCount = 0;

            var recordsList = recordsEnumerable.ToArray();
            var recordsLength = recordsList.Length;



            // Add headers as first line

            for (var i = 0; i < recordsLength; i++)
            {
                // Need to cast to Dictionary to be able to get keys kontaining spaces
                IDictionary<string, object> recordDict = (IDictionary<string, object>)recordsList[i];
                foreach (var fieldName in targettingFields)
                {
                    try
                    {
                        var sanitizeString = recordDict[fieldName]?.ToString() ?? string.Empty;

                        if (string.IsNullOrWhiteSpace(sanitizeString)) continue;

                        var sampleData = new SentimentModel.ModelInput()
                        {
                            Tried = sanitizeString,
                        };
                        //Make a single prediction on the sample data
                        var predictionResult = SentimentModel.Predict(sampleData);
                        if (predictionResult.PredictedLabel)
                        {
                            hitCount += 1;
                            recordDict[fieldName] = replaceWith;
                        }
                    }
                    catch (Exception)
                    {
                        break;
                    }

                    //if (debugLevel == DebugLevel.VerboseLogging)
                    //{
                    //    progressCount += 1;
                    //    var newProgress = Math.Floor((double)progressCount / recordsLength * 100) / 100;
                    //    if (newProgress != actualProgress)
                    //    {
                    //        actualProgress = newProgress;
                    //        if ((actualProgress * 100) % 10 == 0)
                    //            Console.WriteLine($"Progress {progressCount} av {recordsLength}: {actualProgress * 100}%");
                    //    }
                    //}
                    //recordsList[i] = recordDict; // trengs denne...
                }
            }

            anyThingReplaced = hitCount > 0;
            return recordsList;
        }
    }
}

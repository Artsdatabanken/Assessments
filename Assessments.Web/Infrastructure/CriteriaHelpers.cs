namespace Assessments.Web.Infrastructure
{
    public static class CriteriaHelpers
    {
        public static string handleCCriteria(string current_element, string key)
        {
            if (key.Contains("C"))
            {
                // THIS IS ONLY RELEVANT FOR C2
                current_element = current_element.Replace("a(i,ii)", "a(i)a(ii)");
                current_element = current_element.Replace(")", ");");
                current_element = current_element.Replace(" ", "");
            }
            else
            {
                current_element = current_element.Replace(",", ";");
            }
            return current_element;
        }

        public static string removeOuterParenthesis(string wrapped)
        {
            if (wrapped.Length > 2)
            {
                if (wrapped.StartsWith("(") && wrapped.EndsWith(")"))
                {
                    return wrapped.Substring(1, wrapped.Length - 2);
                }
            }
            return wrapped;
        }

        public static string inString(string element, string bigstring)
        {
            if (bigstring.Contains(element))
            {
                return "active";
            }
            return "inactive";
        }

        public static string inList(string element, string[] bigstring)
        {
            foreach (string el in bigstring)
            {
                if (el == element)
                {
                    return "active";
                }
            }
            return "inactive";
        }

        public static string[] obtainSubCriteriaList(string[] splitlist, string k)
        {
            var current_element = "";
            foreach(string element in splitlist)
        {
                if (element.Contains(k))// k = current criteria
                {
                    current_element = element;
                }
            }
            current_element = current_element.Replace(k, ""); // remove main criteria, leaving nested subcriteria
            current_element = CriteriaHelpers.removeOuterParenthesis(current_element);
            current_element = CriteriaHelpers.handleCCriteria(current_element, k);
            return current_element.Split(";");
        }

        public static string subCriteria(string element, string bigstring)
        {
            return bigstring.Replace("+", ";" + element);
        }

        public static string sortBCriteria(string b)
        {
            string b_subcriteria = "";

            b = CriteriaHelpers.subCriteria("B", b);
            if (b.Contains("B1"))
            {
                b_subcriteria += "B1;";
                b = b.Replace("B1", "");
            }
            if (b.Contains("B2"))
            {
                b_subcriteria += "B2";
                b = b.Replace("B2", "");
            }

            b = b.Replace("b", ";b");
            b = b.Replace("c", ";c");

            var b_distinctlist = b.Split(";").Distinct();
            string b_options = string.Join(";", b_distinctlist);
            b = b_subcriteria + ";+" + b_options;

            return b;
        }

        public static Dictionary<string, string> sortCriteria(string criteria)
        {
            string a = "", b = "", c = "", d = "";

            void placeCriteria(string criteria)
            {
                if (!string.IsNullOrEmpty(criteria))
                {
                    if (criteria.Contains("A"))
                    {
                        a = criteria;
                    }
                    else if (criteria.Contains("B"))
                    {
                        b = criteria;
                    }
                    else if (criteria.Contains("C"))
                    {
                        c = criteria;
                    }
                    else if (criteria.Contains("D"))
                    {
                        d = criteria;
                    }
                }
            }

            // Iterate the criteria string and place each separate criteria in its correct container.
            if (!string.IsNullOrEmpty(criteria) && criteria.Contains(";"))
            {
                // SPLIT THE LIST AND LOOP THEM
                foreach (string crit in criteria.Split(";"))
                {
                    placeCriteria(crit);
                }
            }
            else
            {
                placeCriteria(criteria);
            }

            // Sort criteria contents
            a = CriteriaHelpers.subCriteria("A", a); // A CRITERIA - OPTIONS AND SUBCRITERIA ARE NOT SEPARATE ENTITIES
            c = CriteriaHelpers.subCriteria("C", c); // C CRITERIA - All options are only relevant for C2 - handle in code.
            d = CriteriaHelpers.subCriteria("D", d); // D CRITERIA: - ONLY subcriteria.
            b = CriteriaHelpers.sortBCriteria(b);    // B CRITERIA - OPTIONS AND SUBCRITERIA ARE SEPARATE ENTITIES, options may contain sub-options

            // The dictionaries used in the view <- two dicts made sense back when A was thought to have options as well. Now not as much

            var subcriteria = new Dictionary<string, string>(){
            {"A", a},
            {"B", b},
            {"C", c},
            {"D", d}
            };

            return subcriteria;
        }

    }
}
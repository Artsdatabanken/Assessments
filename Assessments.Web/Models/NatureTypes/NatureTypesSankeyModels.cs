namespace Assessments.Web.Models.NatureTypes
{
    public class Node
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public string Category { get; set; }

        public string Type { get; set; }

        public int Source { get; set; }

        public int Target { get; set; }
    }

    public class Link
    {
        public int Id { get; set; }

        public int Source { get; set; }

        public int Target { get; set; }

        public int Value { get; set; }
    }
}
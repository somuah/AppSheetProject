namespace AppSheetProject
{
    class Person
    {
        public int id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public string number { get; set; }
        public string photo { get; set; }
        public string bio { get; set; }

        public override string ToString()
        {
            return "id: " + this.id + " name: " + this.name + " age: " + this.age;
        }
    }
}

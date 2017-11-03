using System;

namespace Bluetooth2._0Demo
{
    class Student
    {
        public string Name { get; set; }
        private string _id;
        public string Time { get; set; }

        public string Id { get; set; }
        public string Picture { get; set; }
        public string Mac { get; set; }

        public string Path
        {
            get { return _id; }
            set
            {
                _id = value;
                Picture = AppDomain.CurrentDomain.BaseDirectory + "Image/" + _id + ".jpg";
            }
        }
    }
}

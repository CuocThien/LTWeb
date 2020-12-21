using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace demo.Model
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            this.Label = label;
            this.Y = y;
        }
        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;

    }
    [DataContract]
    public class DataPointLine
    {

        public DataPointLine(double x, double y)
        {
            this.x = x;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "x")]
        public Nullable<double> x = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }
}
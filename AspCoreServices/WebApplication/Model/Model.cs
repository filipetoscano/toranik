using Ohm;
using System;

namespace Toranik.Endpoint
{
    /// <summary>
    /// Class that has all of the 'base' types that we support.
    /// </summary>
    public class Native
    {
        public bool Boolean;

        public short Short;
        public int Integer;
        public long Long;
        public float Float;
        public double Double;
        public decimal Decimal;

        public DateTime DateTime;
        public string String;
        public Uri Uri;
    }


    /// <summary>
    /// Class that has nullables of base types. If the type is a value
    /// type, then uses Nullable<> generic, otherwise the field/property
    /// needs to be marked with the [Optional] attribute.
    /// </summary>
    public class NativeOptional
    {
        public bool? Boolean;

        public short? Short;
        public int? Integer;
        public long? Long;
        public float? Float;
        public double? Double;
        public decimal? Decimal;

        public DateTime? DateTime;

        [Optional]
        public string String;

        [Optional]
        public Uri Uri;
    }


    /// <summary>
    /// Normal enumerate.
    /// </summary>
    public enum EnumerateNormal
    {
        None = 0,
        One = 1,
        Two = 2,
        Three = 3
    }


    /// <summary>
    /// Flaggable enumerate.
    /// </summary>
    [Flags]
    public enum EnumerateFlags
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 4
    }


    /// <summary>
    /// Enumerates.
    /// </summary>
    public class Enumerates
    {
        public EnumerateNormal Normal;
        public EnumerateNormal[] NormalArray;
        public EnumerateFlags Flags;
        public EnumerateFlags[] FlagsArray;
    }


    /// <summary>
    /// Class that has both properties and fields: both need to be
    /// identified as content of the model.
    /// </summary>
    public class PropertyVsField
    {
        public string Property { get; set; }
        public string Field;
    }


    /// <summary>
    /// Class that has array of native types.
    /// </summary>
    public class NativeArray
    {
        public bool[] Boolean;

        public short[] Short;
        public int[] Integer;
        public long[] Long;
        public float[] Float;
        public double[] Double;
        public decimal[] Decimal;

        public DateTime[] DateTime;
        public string[] String;
        public Uri[] Uri;
    }


    /// <summary>
    /// Class that references itself. Has both a single
    /// and array version of itself.
    /// </summary>
    public class Recursive
    {
        public Recursive Recurse;
        public Recursive[] RecurseArray;
    }


    /// <summary>
    /// Class not explicitly used by the API, but referenced by
    /// a class which has been referenced by the API.
    /// </summary>
    public class Depth2
    {
        public string Name;
    }


    /// <summary>
    /// Class not explicitly used by API, but referenced by one.
    /// </summary>
    public class Depth1
    {
        public string Name;
        public Depth2 Depth;
    }


    /// <summary>
    /// Class that is used by API, but that references other classes
    /// which are not used (explicitly) by the API. These must also
    /// be included into the contract description.
    /// </summary>
    public class Depth0
    {
        public string Name;
        public Depth1 Depth;
    }
}

using System;
using System.Collections;


namespace UtilityLibrary.Serialization
{
  /// <summary>
  /// Interface which can be used by clients to serialize values
  /// </summary>
  public interface ISerializator
  {
    /// <summary>
    /// Save value into storage with unknown to interface type. In serialization
    /// used value.ToString() method.
    /// </summary>
    /// <param name="name">Name of serialization property</param>
    /// <param name="value">value of serialized property</param>
    void AddValue( string name, object value );
    /// <summary>
    /// Save Integer value into storage
    /// </summary>
    /// <param name="name">Name of serialization property</param>
    /// <param name="value">value of serialized property</param>
    void AddValue( string name, int value );
    /// <summary>
    /// Save Boolean value into storage
    /// </summary>
    /// <param name="name">Name of serialization property</param>
    /// <param name="value">value of serialized property</param>
    void AddValue( string name, bool value );
    /// <summary>
    /// Save Array of values into storage
    /// </summary>
    /// <param name="name">Name of serialization property</param>
    /// <param name="value">value of serialized property</param>
    void AddValue( string name, Array value );
    /// <summary>
    /// Save DateTime object into storage
    /// </summary>
    /// <param name="name">Name of serialization property</param>
    /// <param name="value">value of serialized property</param>
    void AddValue( string name, DateTime value );
    /// <summary>
    /// Save string value into storage
    /// </summary>
    /// <param name="name">Name of serialization property</param>
    /// <param name="value">value of serialized property</param>
    void AddValue( string name, string value );
    /// <summary>
    /// Save double value into storage
    /// </summary>
    /// <param name="name">Name of serialization property</param>
    /// <param name="value">value of serialized property</param>
    void AddValue( string name, double value );
    /// <summary>
    /// Return array of objects. Datatype of object is the same as used
    /// for saving. (in most cases work only for standard items which has
    /// public static method Parse)
    /// </summary>
    /// <param name="name">Storage property name</param>
    Array GetArrayValue( string name );
    /// <summary>
    /// Return array of objects. Datatype of object is the same as used
    /// for saving. (in most cases work only for standard items which has
    /// public static method Parse)
    /// </summary>
    /// <param name="name">Storage property name</param>
    /// <param name="defValue">User specifed default value. Used in cases 
    /// when property with specified name does not exist in storage</param>
    Array GetArrayValue( string name, Array defValue );
    /// <summary>
    /// Get from storage Integer value
    /// </summary>
    /// <param name="name">Storage property name</param>
    int GetIntValue( string name );
    /// <summary>
    /// Get from storage Integer value
    /// </summary>
    /// <param name="name">Storage property name</param>
    /// <param name="defValue">User specifed default value. Used in cases 
    /// when property with specified name does not exist in storage</param>
    int GetIntValue( string name, int defValue );
    /// <summary>
    /// Get from storage boolean value
    /// </summary>
    /// <param name="name">Storage property name</param>
    bool GetBoolValue( string name );
    /// <summary>
    /// Get from storage boolean value
    /// </summary>
    /// <param name="name">Storage property name</param>
    /// <param name="defValue">User specifed default value. Used in cases 
    /// when property with specified name does not exist in storage</param>
    bool GetBoolValue( string name, bool defValue );
    /// <summary>
    /// Get DateTime object from storage
    /// </summary>
    /// <param name="name">Storage property name</param>
    DateTime GetDateTimeValue( string name );
    /// <summary>
    /// Get DateTime object from storage
    /// </summary>
    /// <param name="name">Storage property name</param>
    /// <param name="defValue">User specifed default value. Used in cases 
    /// when property with specified name does not exist in storage</param>
    DateTime GetDateTimeValue( string name, DateTime defValue );
    /// <summary>
    /// Get String value from storage
    /// </summary>
    /// <param name="name">Storage property name</param>
    string GetStringValue( string name );
    /// <summary>
    /// Get String value from storage
    /// </summary>
    /// <param name="name">Storage property name</param>
    /// <param name="defValue">User specifed default value. Used in cases 
    /// when property with specified name does not exist in storage</param>
    string GetStringValue( string name, string defValue );
    /// <summary>
    /// Get Double value from storage
    /// </summary>
    /// <param name="name">Storage property name</param>
    double GetDoubleValue( string name );
    /// <summary>
    /// Get Double value from storage
    /// </summary>
    /// <param name="name">Storage property name</param>
    /// <param name="defValue">User specifed default value. Used in cases 
    /// when property with specified name does not exist in storage</param>
    double GetDoubleValue( string name, double defValue );
    /// <summary>
    /// Get Unknown to storage datatype.
    /// </summary>
    /// <param name="name">Storage property name</param>
    object GetValue( string name );
    /// <summary>
    /// Get Unknown to storage datatype.
    /// </summary>
    /// <param name="name">Storage property name</param>
    /// <param name="defValue">User specifed default value. Used in cases 
    /// when property with specified name does not exist in storage</param>
    object GetValue( string name, object defValue );
  }

  /// <summary>
  /// Interface used by class Serializator main object.
  /// </summary>
  public interface IRootSerializator : ISerializator, IDisposable
  {
    /// <summary>
    /// Enforce saving current XML into file
    /// </summary>
    void Save();
    /// <summary>
    /// Specify into which subkey values will be stored
    /// </summary>
    string Subkey{ get; set; }
    /// <summary>
    /// Get Client serialization interface Subkey for client interface
    /// is equel to subkey indexer path-string.
    /// </summary>
    ISerializator this[ string subkey ]{ get; }
  }
}

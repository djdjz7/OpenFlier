// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: enums/GraphEnum.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from enums/GraphEnum.proto</summary>
public static partial class GraphEnumReflection {

  #region Descriptor
  /// <summary>File descriptor for enums/GraphEnum.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static GraphEnumReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChVlbnVtcy9HcmFwaEVudW0ucHJvdG8qhgEKCUdyYXBoRW51bRIbChdCT1VO",
          "REVEX0NPTlRBSU5FUl9HUkFQSBAAEg8KC0dST1VQX0dSQVBIEAESDwoLSU1B",
          "R0VfR1JBUEgQAhIQCgxTVFJPS0VfR1JBUEgQAxIYChRCRUVMSU5FX1NUUk9L",
          "RV9HUkFQSBAEEg4KClRFWFRfR1JBUEgQBUIeCg5jb20uenlrai5ib2FyZEIM",
          "RXp5R3JhcGhFbnVtYgZwcm90bzM="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(new[] {typeof(global::GraphEnum), }, null, null));
  }
  #endregion

}
#region Enums
public enum GraphEnum {
  [pbr::OriginalName("BOUNDED_CONTAINER_GRAPH")] BoundedContainerGraph = 0,
  [pbr::OriginalName("GROUP_GRAPH")] GroupGraph = 1,
  [pbr::OriginalName("IMAGE_GRAPH")] ImageGraph = 2,
  [pbr::OriginalName("STROKE_GRAPH")] StrokeGraph = 3,
  [pbr::OriginalName("BEELINE_STROKE_GRAPH")] BeelineStrokeGraph = 4,
  [pbr::OriginalName("TEXT_GRAPH")] TextGraph = 5,
}

#endregion


#endregion Designer generated code

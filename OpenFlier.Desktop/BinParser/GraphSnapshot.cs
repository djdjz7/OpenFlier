// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: GraphSnapshot.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from GraphSnapshot.proto</summary>
public static partial class GraphSnapshotReflection {

  #region Descriptor
  /// <summary>File descriptor for GraphSnapshot.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static GraphSnapshotReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChNHcmFwaFNuYXBzaG90LnByb3RvGhF1dGlscy9SZWN0Ri5wcm90bxoRdXRp",
          "bHMvUGFpbnQucHJvdG8aEnV0aWxzL01hdHJpeC5wcm90bxoVZW51bXMvR3Jh",
          "cGhFbnVtLnByb3RvIuwCCgxQYWdlU25hcHNob3QSHQoMY2FtZXJhTWF0cml4",
          "GAEgASgLMgcuTWF0cml4EjIKDWdyYXBoU25hcHNob3QYAiABKAsyGy5QYWdl",
          "U25hcHNob3QuR3JhcGhTbmFwc2hvdBqIAgoNR3JhcGhTbmFwc2hvdBIKCgJp",
          "ZBgBIAEoCRIdCglncmFwaEVudW0YAiABKA4yCi5HcmFwaEVudW0SFwoHb3V0",
          "UmVjdBgDIAEoCzIGLlJlY3RGEhcKBm1hdHJpeBgEIAEoCzIHLk1hdHJpeBIv",
          "CgpjaGlsZEdyYXBoGAUgAygLMhsuUGFnZVNuYXBzaG90LkdyYXBoU25hcHNo",
          "b3QSFgoOc291cmNlRmlsZU5hbWUYBiABKAkSEAoIc291cmNlSWQYByABKAUS",
          "FwoPYmFja2dyb3VuZENvbG9yGAggASgFEhUKBXBhaW50GAkgASgLMgYuUGFp",
          "bnQSDwoHY29udGVudBgKIAEoCUIhCg5jb20uenlrai5ib2FyZEIPRXp5UGFn",
          "ZVNuYXBzaG90YgZwcm90bzM="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { global::RectFReflection.Descriptor, global::PaintReflection.Descriptor, global::MatrixReflection.Descriptor, global::GraphEnumReflection.Descriptor, },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::PageSnapshot), global::PageSnapshot.Parser, new[]{ "CameraMatrix", "GraphSnapshot" }, null, null, null, new pbr::GeneratedClrTypeInfo[] { new pbr::GeneratedClrTypeInfo(typeof(global::PageSnapshot.Types.GraphSnapshot), global::PageSnapshot.Types.GraphSnapshot.Parser, new[]{ "Id", "GraphEnum", "OutRect", "Matrix", "ChildGraph", "SourceFileName", "SourceId", "BackgroundColor", "Paint", "Content" }, null, null, null, null)})
        }));
  }
  #endregion

}
#region Messages
public sealed partial class PageSnapshot : pb::IMessage<PageSnapshot>
#if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    , pb::IBufferMessage
#endif
{
  private static readonly pb::MessageParser<PageSnapshot> _parser = new pb::MessageParser<PageSnapshot>(() => new PageSnapshot());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pb::MessageParser<PageSnapshot> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::GraphSnapshotReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PageSnapshot() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PageSnapshot(PageSnapshot other) : this() {
    cameraMatrix_ = other.cameraMatrix_ != null ? other.cameraMatrix_.Clone() : null;
    graphSnapshot_ = other.graphSnapshot_ != null ? other.graphSnapshot_.Clone() : null;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public PageSnapshot Clone() {
    return new PageSnapshot(this);
  }

  /// <summary>Field number for the "cameraMatrix" field.</summary>
  public const int CameraMatrixFieldNumber = 1;
  private global::Matrix cameraMatrix_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public global::Matrix CameraMatrix {
    get { return cameraMatrix_; }
    set {
      cameraMatrix_ = value;
    }
  }

  /// <summary>Field number for the "graphSnapshot" field.</summary>
  public const int GraphSnapshotFieldNumber = 2;
  private global::PageSnapshot.Types.GraphSnapshot graphSnapshot_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public global::PageSnapshot.Types.GraphSnapshot GraphSnapshot {
    get { return graphSnapshot_; }
    set {
      graphSnapshot_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override bool Equals(object other) {
    return Equals(other as PageSnapshot);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public bool Equals(PageSnapshot other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (!object.Equals(CameraMatrix, other.CameraMatrix)) return false;
    if (!object.Equals(GraphSnapshot, other.GraphSnapshot)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override int GetHashCode() {
    int hash = 1;
    if (cameraMatrix_ != null) hash ^= CameraMatrix.GetHashCode();
    if (graphSnapshot_ != null) hash ^= GraphSnapshot.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void WriteTo(pb::CodedOutputStream output) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    output.WriteRawMessage(this);
  #else
    if (cameraMatrix_ != null) {
      output.WriteRawTag(10);
      output.WriteMessage(CameraMatrix);
    }
    if (graphSnapshot_ != null) {
      output.WriteRawTag(18);
      output.WriteMessage(GraphSnapshot);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
    if (cameraMatrix_ != null) {
      output.WriteRawTag(10);
      output.WriteMessage(CameraMatrix);
    }
    if (graphSnapshot_ != null) {
      output.WriteRawTag(18);
      output.WriteMessage(GraphSnapshot);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(ref output);
    }
  }
  #endif

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public int CalculateSize() {
    int size = 0;
    if (cameraMatrix_ != null) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(CameraMatrix);
    }
    if (graphSnapshot_ != null) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(GraphSnapshot);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(PageSnapshot other) {
    if (other == null) {
      return;
    }
    if (other.cameraMatrix_ != null) {
      if (cameraMatrix_ == null) {
        CameraMatrix = new global::Matrix();
      }
      CameraMatrix.MergeFrom(other.CameraMatrix);
    }
    if (other.graphSnapshot_ != null) {
      if (graphSnapshot_ == null) {
        GraphSnapshot = new global::PageSnapshot.Types.GraphSnapshot();
      }
      GraphSnapshot.MergeFrom(other.GraphSnapshot);
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public void MergeFrom(pb::CodedInputStream input) {
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    input.ReadRawMessage(this);
  #else
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          if (cameraMatrix_ == null) {
            CameraMatrix = new global::Matrix();
          }
          input.ReadMessage(CameraMatrix);
          break;
        }
        case 18: {
          if (graphSnapshot_ == null) {
            GraphSnapshot = new global::PageSnapshot.Types.GraphSnapshot();
          }
          input.ReadMessage(GraphSnapshot);
          break;
        }
      }
    }
  #endif
  }

  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
          break;
        case 10: {
          if (cameraMatrix_ == null) {
            CameraMatrix = new global::Matrix();
          }
          input.ReadMessage(CameraMatrix);
          break;
        }
        case 18: {
          if (graphSnapshot_ == null) {
            GraphSnapshot = new global::PageSnapshot.Types.GraphSnapshot();
          }
          input.ReadMessage(GraphSnapshot);
          break;
        }
      }
    }
  }
  #endif

  #region Nested types
  /// <summary>Container for nested types declared in the PageSnapshot message type.</summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
  public static partial class Types {
    public sealed partial class GraphSnapshot : pb::IMessage<GraphSnapshot>
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        , pb::IBufferMessage
    #endif
    {
      private static readonly pb::MessageParser<GraphSnapshot> _parser = new pb::MessageParser<GraphSnapshot>(() => new GraphSnapshot());
      private pb::UnknownFieldSet _unknownFields;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public static pb::MessageParser<GraphSnapshot> Parser { get { return _parser; } }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public static pbr::MessageDescriptor Descriptor {
        get { return global::PageSnapshot.Descriptor.NestedTypes[0]; }
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      pbr::MessageDescriptor pb::IMessage.Descriptor {
        get { return Descriptor; }
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public GraphSnapshot() {
        OnConstruction();
      }

      partial void OnConstruction();

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public GraphSnapshot(GraphSnapshot other) : this() {
        id_ = other.id_;
        graphEnum_ = other.graphEnum_;
        outRect_ = other.outRect_ != null ? other.outRect_.Clone() : null;
        matrix_ = other.matrix_ != null ? other.matrix_.Clone() : null;
        childGraph_ = other.childGraph_.Clone();
        sourceFileName_ = other.sourceFileName_;
        sourceId_ = other.sourceId_;
        backgroundColor_ = other.backgroundColor_;
        paint_ = other.paint_ != null ? other.paint_.Clone() : null;
        content_ = other.content_;
        _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public GraphSnapshot Clone() {
        return new GraphSnapshot(this);
      }

      /// <summary>Field number for the "id" field.</summary>
      public const int IdFieldNumber = 1;
      private string id_ = "";
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public string Id {
        get { return id_; }
        set {
          id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
      }

      /// <summary>Field number for the "graphEnum" field.</summary>
      public const int GraphEnumFieldNumber = 2;
      private global::GraphEnum graphEnum_ = global::GraphEnum.BoundedContainerGraph;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public global::GraphEnum GraphEnum {
        get { return graphEnum_; }
        set {
          graphEnum_ = value;
        }
      }

      /// <summary>Field number for the "outRect" field.</summary>
      public const int OutRectFieldNumber = 3;
      private global::RectF outRect_;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public global::RectF OutRect {
        get { return outRect_; }
        set {
          outRect_ = value;
        }
      }

      /// <summary>Field number for the "matrix" field.</summary>
      public const int MatrixFieldNumber = 4;
      private global::Matrix matrix_;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public global::Matrix Matrix {
        get { return matrix_; }
        set {
          matrix_ = value;
        }
      }

      /// <summary>Field number for the "childGraph" field.</summary>
      public const int ChildGraphFieldNumber = 5;
      private static readonly pb::FieldCodec<global::PageSnapshot.Types.GraphSnapshot> _repeated_childGraph_codec
          = pb::FieldCodec.ForMessage(42, global::PageSnapshot.Types.GraphSnapshot.Parser);
      private readonly pbc::RepeatedField<global::PageSnapshot.Types.GraphSnapshot> childGraph_ = new pbc::RepeatedField<global::PageSnapshot.Types.GraphSnapshot>();
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public pbc::RepeatedField<global::PageSnapshot.Types.GraphSnapshot> ChildGraph {
        get { return childGraph_; }
      }

      /// <summary>Field number for the "sourceFileName" field.</summary>
      public const int SourceFileNameFieldNumber = 6;
      private string sourceFileName_ = "";
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public string SourceFileName {
        get { return sourceFileName_; }
        set {
          sourceFileName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
      }

      /// <summary>Field number for the "sourceId" field.</summary>
      public const int SourceIdFieldNumber = 7;
      private int sourceId_;
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public int SourceId {
        get { return sourceId_; }
        set {
          sourceId_ = value;
        }
      }

      /// <summary>Field number for the "backgroundColor" field.</summary>
      public const int BackgroundColorFieldNumber = 8;
      private int backgroundColor_;
      /// <summary>
      /// 背景色
      /// </summary>
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public int BackgroundColor {
        get { return backgroundColor_; }
        set {
          backgroundColor_ = value;
        }
      }

      /// <summary>Field number for the "paint" field.</summary>
      public const int PaintFieldNumber = 9;
      private global::Paint paint_;
      /// <summary>
      /// 画笔
      /// </summary>
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public global::Paint Paint {
        get { return paint_; }
        set {
          paint_ = value;
        }
      }

      /// <summary>Field number for the "content" field.</summary>
      public const int ContentFieldNumber = 10;
      private string content_ = "";
      /// <summary>
      /// 文本内容
      /// </summary>
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public string Content {
        get { return content_; }
        set {
          content_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
        }
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public override bool Equals(object other) {
        return Equals(other as GraphSnapshot);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public bool Equals(GraphSnapshot other) {
        if (ReferenceEquals(other, null)) {
          return false;
        }
        if (ReferenceEquals(other, this)) {
          return true;
        }
        if (Id != other.Id) return false;
        if (GraphEnum != other.GraphEnum) return false;
        if (!object.Equals(OutRect, other.OutRect)) return false;
        if (!object.Equals(Matrix, other.Matrix)) return false;
        if(!childGraph_.Equals(other.childGraph_)) return false;
        if (SourceFileName != other.SourceFileName) return false;
        if (SourceId != other.SourceId) return false;
        if (BackgroundColor != other.BackgroundColor) return false;
        if (!object.Equals(Paint, other.Paint)) return false;
        if (Content != other.Content) return false;
        return Equals(_unknownFields, other._unknownFields);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public override int GetHashCode() {
        int hash = 1;
        if (Id.Length != 0) hash ^= Id.GetHashCode();
        if (GraphEnum != global::GraphEnum.BoundedContainerGraph) hash ^= GraphEnum.GetHashCode();
        if (outRect_ != null) hash ^= OutRect.GetHashCode();
        if (matrix_ != null) hash ^= Matrix.GetHashCode();
        hash ^= childGraph_.GetHashCode();
        if (SourceFileName.Length != 0) hash ^= SourceFileName.GetHashCode();
        if (SourceId != 0) hash ^= SourceId.GetHashCode();
        if (BackgroundColor != 0) hash ^= BackgroundColor.GetHashCode();
        if (paint_ != null) hash ^= Paint.GetHashCode();
        if (Content.Length != 0) hash ^= Content.GetHashCode();
        if (_unknownFields != null) {
          hash ^= _unknownFields.GetHashCode();
        }
        return hash;
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public override string ToString() {
        return pb::JsonFormatter.ToDiagnosticString(this);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public void WriteTo(pb::CodedOutputStream output) {
      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        output.WriteRawMessage(this);
      #else
        if (Id.Length != 0) {
          output.WriteRawTag(10);
          output.WriteString(Id);
        }
        if (GraphEnum != global::GraphEnum.BoundedContainerGraph) {
          output.WriteRawTag(16);
          output.WriteEnum((int) GraphEnum);
        }
        if (outRect_ != null) {
          output.WriteRawTag(26);
          output.WriteMessage(OutRect);
        }
        if (matrix_ != null) {
          output.WriteRawTag(34);
          output.WriteMessage(Matrix);
        }
        childGraph_.WriteTo(output, _repeated_childGraph_codec);
        if (SourceFileName.Length != 0) {
          output.WriteRawTag(50);
          output.WriteString(SourceFileName);
        }
        if (SourceId != 0) {
          output.WriteRawTag(56);
          output.WriteInt32(SourceId);
        }
        if (BackgroundColor != 0) {
          output.WriteRawTag(64);
          output.WriteInt32(BackgroundColor);
        }
        if (paint_ != null) {
          output.WriteRawTag(74);
          output.WriteMessage(Paint);
        }
        if (Content.Length != 0) {
          output.WriteRawTag(82);
          output.WriteString(Content);
        }
        if (_unknownFields != null) {
          _unknownFields.WriteTo(output);
        }
      #endif
      }

      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
        if (Id.Length != 0) {
          output.WriteRawTag(10);
          output.WriteString(Id);
        }
        if (GraphEnum != global::GraphEnum.BoundedContainerGraph) {
          output.WriteRawTag(16);
          output.WriteEnum((int) GraphEnum);
        }
        if (outRect_ != null) {
          output.WriteRawTag(26);
          output.WriteMessage(OutRect);
        }
        if (matrix_ != null) {
          output.WriteRawTag(34);
          output.WriteMessage(Matrix);
        }
        childGraph_.WriteTo(ref output, _repeated_childGraph_codec);
        if (SourceFileName.Length != 0) {
          output.WriteRawTag(50);
          output.WriteString(SourceFileName);
        }
        if (SourceId != 0) {
          output.WriteRawTag(56);
          output.WriteInt32(SourceId);
        }
        if (BackgroundColor != 0) {
          output.WriteRawTag(64);
          output.WriteInt32(BackgroundColor);
        }
        if (paint_ != null) {
          output.WriteRawTag(74);
          output.WriteMessage(Paint);
        }
        if (Content.Length != 0) {
          output.WriteRawTag(82);
          output.WriteString(Content);
        }
        if (_unknownFields != null) {
          _unknownFields.WriteTo(ref output);
        }
      }
      #endif

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public int CalculateSize() {
        int size = 0;
        if (Id.Length != 0) {
          size += 1 + pb::CodedOutputStream.ComputeStringSize(Id);
        }
        if (GraphEnum != global::GraphEnum.BoundedContainerGraph) {
          size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) GraphEnum);
        }
        if (outRect_ != null) {
          size += 1 + pb::CodedOutputStream.ComputeMessageSize(OutRect);
        }
        if (matrix_ != null) {
          size += 1 + pb::CodedOutputStream.ComputeMessageSize(Matrix);
        }
        size += childGraph_.CalculateSize(_repeated_childGraph_codec);
        if (SourceFileName.Length != 0) {
          size += 1 + pb::CodedOutputStream.ComputeStringSize(SourceFileName);
        }
        if (SourceId != 0) {
          size += 1 + pb::CodedOutputStream.ComputeInt32Size(SourceId);
        }
        if (BackgroundColor != 0) {
          size += 1 + pb::CodedOutputStream.ComputeInt32Size(BackgroundColor);
        }
        if (paint_ != null) {
          size += 1 + pb::CodedOutputStream.ComputeMessageSize(Paint);
        }
        if (Content.Length != 0) {
          size += 1 + pb::CodedOutputStream.ComputeStringSize(Content);
        }
        if (_unknownFields != null) {
          size += _unknownFields.CalculateSize();
        }
        return size;
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public void MergeFrom(GraphSnapshot other) {
        if (other == null) {
          return;
        }
        if (other.Id.Length != 0) {
          Id = other.Id;
        }
        if (other.GraphEnum != global::GraphEnum.BoundedContainerGraph) {
          GraphEnum = other.GraphEnum;
        }
        if (other.outRect_ != null) {
          if (outRect_ == null) {
            OutRect = new global::RectF();
          }
          OutRect.MergeFrom(other.OutRect);
        }
        if (other.matrix_ != null) {
          if (matrix_ == null) {
            Matrix = new global::Matrix();
          }
          Matrix.MergeFrom(other.Matrix);
        }
        childGraph_.Add(other.childGraph_);
        if (other.SourceFileName.Length != 0) {
          SourceFileName = other.SourceFileName;
        }
        if (other.SourceId != 0) {
          SourceId = other.SourceId;
        }
        if (other.BackgroundColor != 0) {
          BackgroundColor = other.BackgroundColor;
        }
        if (other.paint_ != null) {
          if (paint_ == null) {
            Paint = new global::Paint();
          }
          Paint.MergeFrom(other.Paint);
        }
        if (other.Content.Length != 0) {
          Content = other.Content;
        }
        _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
      }

      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      public void MergeFrom(pb::CodedInputStream input) {
      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
        input.ReadRawMessage(this);
      #else
        uint tag;
        while ((tag = input.ReadTag()) != 0) {
          switch(tag) {
            default:
              _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
              break;
            case 10: {
              Id = input.ReadString();
              break;
            }
            case 16: {
              GraphEnum = (global::GraphEnum) input.ReadEnum();
              break;
            }
            case 26: {
              if (outRect_ == null) {
                OutRect = new global::RectF();
              }
              input.ReadMessage(OutRect);
              break;
            }
            case 34: {
              if (matrix_ == null) {
                Matrix = new global::Matrix();
              }
              input.ReadMessage(Matrix);
              break;
            }
            case 42: {
              childGraph_.AddEntriesFrom(input, _repeated_childGraph_codec);
              break;
            }
            case 50: {
              SourceFileName = input.ReadString();
              break;
            }
            case 56: {
              SourceId = input.ReadInt32();
              break;
            }
            case 64: {
              BackgroundColor = input.ReadInt32();
              break;
            }
            case 74: {
              if (paint_ == null) {
                Paint = new global::Paint();
              }
              input.ReadMessage(Paint);
              break;
            }
            case 82: {
              Content = input.ReadString();
              break;
            }
          }
        }
      #endif
      }

      #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
      [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
      void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
        uint tag;
        while ((tag = input.ReadTag()) != 0) {
          switch(tag) {
            default:
              _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
              break;
            case 10: {
              Id = input.ReadString();
              break;
            }
            case 16: {
              GraphEnum = (global::GraphEnum) input.ReadEnum();
              break;
            }
            case 26: {
              if (outRect_ == null) {
                OutRect = new global::RectF();
              }
              input.ReadMessage(OutRect);
              break;
            }
            case 34: {
              if (matrix_ == null) {
                Matrix = new global::Matrix();
              }
              input.ReadMessage(Matrix);
              break;
            }
            case 42: {
              childGraph_.AddEntriesFrom(ref input, _repeated_childGraph_codec);
              break;
            }
            case 50: {
              SourceFileName = input.ReadString();
              break;
            }
            case 56: {
              SourceId = input.ReadInt32();
              break;
            }
            case 64: {
              BackgroundColor = input.ReadInt32();
              break;
            }
            case 74: {
              if (paint_ == null) {
                Paint = new global::Paint();
              }
              input.ReadMessage(Paint);
              break;
            }
            case 82: {
              Content = input.ReadString();
              break;
            }
          }
        }
      }
      #endif

    }

  }
  #endregion

}

#endregion


#endregion Designer generated code

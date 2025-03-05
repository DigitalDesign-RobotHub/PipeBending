using OCCTK.OCC.Topo;

namespace PipeBendingBLL.Model.Pipe;
public class PipeShape {
	public TShape TopoShape { get; set; }
	public HashSet<PipeFace> PFaces { get; set; }

	public PipeShape( TShape shape ) { this.TopoShape = shape; this.PFaces = new HashSet<PipeFace>( ); }
}

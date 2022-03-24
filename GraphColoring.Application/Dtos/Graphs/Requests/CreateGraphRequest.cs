namespace GraphColoring.Application.Dtos.Graphs.Requests
{
    public class CreateGraphRequest
    {
        public int[][] AdjacencyMatrix { get; set; }
        public string GraphName { get; set; }
    }
}
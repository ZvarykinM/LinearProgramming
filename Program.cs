using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace MMethod;

class MatrixBuilder
{
    public static Matrix<double> ReadMatrixFromCsv(string NameOfFile = "/home/max/M_method/Matr.csv")
    {
        var A = new List<double[]>();
        using(var Reader = new StreamReader(NameOfFile))
        {
            while(!Reader.EndOfStream)
            {
                var strs = Reader.ReadLine().Split(';');
                var line = new List<double>();
                foreach(var p in strs)
                    line.Add(Convert.ToDouble(p));   
                A.Add(line.ToArray());
            }
        }
        var Matr = new double[A.Count, A[0].Length];
        for(var i = 0; i < A.Count; i++)
            for(var j = 0; j < A[0].Length; j++)
                Matr[i, j] = A[i][j];
        return DenseMatrix.OfArray(Matr);
    }

    public static Matrix<double> MakeMatrixOfRestrictions(Matrix<double> Matr) =>
        Matr.RemoveRow(Matr.RowCount - 1).RemoveColumn(Matr.ColumnCount - 1);

    public static Matrix<double> MakeMatrixWithEvidentBasis(Matrix<double> A)
    {
        var E = DenseMatrix.CreateDiagonal(A.RowCount, A.RowCount, 1.0);
        return Matrix<double>.Build.DenseOfMatrixArray(new Matrix<double>[,]{{A, E}});
    }
    
    public static Matrix<double> MakeSimplexTable(string PathToFile)
    {
        var Acb = ReadMatrixFromCsv(PathToFile);
        var MatrAWithBasis = MakeMatrixWithEvidentBasis(MakeMatrixOfRestrictions(Acb));
        var FuncArray = new List<double>(Acb.Row(Acb.RowCount - 1).ToArray());
        FuncArray.RemoveAt(FuncArray.Count - 1);
        for(var i = 0; i < Acb.RowCount - 1; i++)
            FuncArray.Add(0.0);
        var FunctionalString = DenseVector.OfArray(FuncArray.ToArray());
        var FreeMembsCol = Acb.Column(Acb.ColumnCount - 1);
        var FuncStr = Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[] {FunctionalString});
        var FreeMembColAsMatr = Matrix<double>.Build.DenseOfRowVectors(new Vector<double>[] {FreeMembsCol});
        var SimplexTable = Matrix<double>.Build.DenseOfMatrixArray(new Matrix<double>[,]{{MatrAWithBasis}, {FuncStr}});
        return Matrix<double>.Build.DenseOfMatrixArray(new Matrix<double>[,]{{SimplexTable, FreeMembColAsMatr.Transpose()}});
    }
}

class Program
{
    
    static void Main(string[] args)
    {   
        Console.Write("введите путь к файлу: ");
        Console.WriteLine(MatrixBuilder.MakeSimplexTable(Console.ReadLine()));
    }
}


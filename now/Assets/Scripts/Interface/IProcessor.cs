using System.Collections.Generic;

public interface IProcessor
{
}

public interface IMProcessor : IProcessor
{
    /// <summary>
    /// ���μ��� Update
    /// </summary>
    /// <param name="models"></param>
    public void Update(List<IModel> models);
}

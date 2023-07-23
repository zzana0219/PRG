using System.Collections.Generic;

public interface IProcessor
{
}

public interface IMProcessor : IProcessor
{
    /// <summary>
    /// 프로세스 Update
    /// </summary>
    /// <param name="models"></param>
    public void Update(List<IModel> models);
}

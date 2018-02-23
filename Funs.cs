/// <summary>
/// 删除文件夹
/// </summary>
private void DeleteFolder()
{ 
    try
    {
        var physicalPath = Request.PhysicalApplicationPath.TrimEnd('\\');
        DirectoryInfo dir = new DirectoryInfo(physicalPath + "\\UploadGDBX\\");
        var fileInfo = dir.GetFileSystemInfos();
        foreach (var i in fileInfo)
        {
            if (i is DirectoryInfo)
            {
                var subdir = new DirectoryInfo(i.FullName);
                var name = i.Name;
                if (name.CompareTo(DateTime.Now.ToString("yyyy-MM-dd")) < 0) //删除文件夹
                {
                    subdir.Delete(true);
                }
                else //删除文件
                {
                    System.IO.File.Delete(i.FullName);
                }
            }
        }
    }
    catch (Exception ex)
    {
        throw ex;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using IMKernel.Visualization;
using log4net;
using PipeBending.Model;

namespace PipeBending.Singleton;

public partial class WorkSpaceContextManager : ObservableObject
{
    private static readonly ILog log = LogManager.GetLogger(typeof(WorkSpaceContextManager));
    private const int MAX_HISTORY_COUNT = 4;
    private readonly SortedDictionary<DateTime, WorkSpace> _historicalRecord = new();

    public IReadOnlyDictionary<DateTime, WorkSpace> HistoricalRecord => _historicalRecord;

    [ObservableProperty]
    private WorkSpace currentWorkSpace;

    public WorkSpaceContextManager()
    {
        CurrentWorkSpace = new WorkSpace();
        log.Info("WorkSpaceContextManager initialized");
    }

    partial void OnCurrentWorkSpaceChanged(WorkSpace value)
    {
        // 当WorkSpace改变时，发送消息通知
        WeakReferenceMessenger.Default.Send(new WorkSpaceChangedMessage(value));
    }

    public async Task LoadNewWorkSpace(string filePath)
    {
        var timestamp = DateTime.Now;
        try
        {
            // 保存当前工作空间到历史记录
            SaveToHistory(timestamp);

            // 从文件加载新的工作空间
            CurrentWorkSpace = await LoadWorkSpaceFromFileAsync(filePath);
            log.Info($"从文件加载新Workspace成功: {filePath}");
        }
        catch (Exception ex)
        {
            log.Error($"加载新Workspace失败: {filePath}", ex);
            throw;
        }
    }

    private async Task<WorkSpace> LoadWorkSpaceFromFileAsync(string filePath)
    {
        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // 这里假设使用JSON序列化，您可以根据实际需求修改
            var options = new JsonSerializerOptions { WriteIndented = true };
            var workspace = await JsonSerializer.DeserializeAsync<WorkSpace>(fileStream, options);
            return workspace
                ?? throw new InvalidOperationException("Failed to deserialize workspace");
        }
        catch (Exception ex)
        {
            log.Error($"Failed to load workspace from file: {filePath}", ex);
            throw;
        }
    }

    private void SaveToHistory(DateTime timestamp)
    {
        if (CurrentWorkSpace != null)
        {
            _historicalRecord[timestamp] = CurrentWorkSpace.Clone() as WorkSpace;
            log.Info($"保存WorkSpace历史记录，时间: {timestamp}");

            // 如果历史记录超过最大数量，删除最早的记录
            if (_historicalRecord.Count > MAX_HISTORY_COUNT)
            {
                var oldestKey = _historicalRecord.Keys.First();
                _historicalRecord.Remove(oldestKey);
                log.Info($"删除旧记录，记录于: {oldestKey}");
            }
        }
    }

    public bool RestoreWorkSpace(DateTime timestamp)
    {
        try
        {
            if (_historicalRecord.TryGetValue(timestamp, out WorkSpace historicalWorkSpace))
            {
                CurrentWorkSpace = historicalWorkSpace.Clone() as WorkSpace;
                log.Info($"还原到 {timestamp} 的workspace");
                return true;
            }

            log.Warn($"未找到 {timestamp} 的workspace记录");
            return false;
        }
        catch (Exception ex)
        {
            log.Error($"还原workspace失败: {timestamp}", ex);
            throw;
        }
    }

    public bool RestoreToPreviousWorkSpace()
    {
        try
        {
            var lastRecord = _historicalRecord.LastOrDefault();
            if (lastRecord.Value != null)
            {
                CurrentWorkSpace = lastRecord.Value.Clone() as WorkSpace;
                _historicalRecord.Remove(lastRecord.Key);
                log.Info($"回退到上一个workspace: {lastRecord.Key}");
                return true;
            }

            log.Warn("没有可回退的workspace记录");
            return false;
        }
        catch (Exception ex)
        {
            log.Error("回退到上一个workspace失败", ex);
            throw;
        }
    }

    public async Task SaveWorkSpaceToFile(string filePath)
    {
        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            var options = new JsonSerializerOptions { WriteIndented = true };
            await JsonSerializer.SerializeAsync(fileStream, CurrentWorkSpace, options);
            log.Info($"Workspace保存成功: {filePath}");

            // 保存后更新历史记录
            SaveToHistory(DateTime.Now);
        }
        catch (Exception ex)
        {
            log.Error($"保存Workspace失败: {filePath}", ex);
            throw;
        }
    }

    public IEnumerable<KeyValuePair<DateTime, WorkSpace>> GetRecentWorkSpaces(int count)
    {
        return _historicalRecord.OrderByDescending(x => x.Key).Take(count);
    }

    public void ClearHistory()
    {
        _historicalRecord.Clear();
        log.Info("清除保存的 WorkSpace 历史记录");
    }
}

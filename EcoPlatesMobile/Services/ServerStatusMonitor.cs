using System.Collections.ObjectModel;
using EcoPlatesMobile.Services.Api;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Services.Api
{
    public sealed class ServerStatusMonitor : IDisposable
    {
        private readonly Func<CancellationToken, Task<ResponseServerStatus>> _userChecker;
        private readonly Func<CancellationToken, Task<ResponseServerStatus>> _companyChecker;

        private Func<CancellationToken, Task<ResponseServerStatus>> _activeChecker;
        private System.Threading.Timer? _timer;
        private readonly object _lock = new();

        private ResponseServerStatus? _latest;
        private DateTimeOffset _lastChecked = DateTimeOffset.MinValue;
        private TimeSpan _interval = TimeSpan.FromSeconds(10);
        private bool _isRunning;

        public event EventHandler<ResponseServerStatus>? StatusChanged;

        public ServerStatusMonitor(UserApiService userApi, CompanyApiService companyApi)
        {
            _userChecker = ct => userApi.CheckServerAsync(TimeSpan.FromSeconds(2), ct);
            _companyChecker = ct => companyApi.CheckServerAsync(TimeSpan.FromSeconds(2), ct);
            _activeChecker = _userChecker; // default
        }

        /// <summary>Switch which service we are monitoring.</summary>
        public void SetRole(UserRole role)
        {
            lock (_lock)
            {
                _activeChecker = role == UserRole.Company ? _companyChecker : _userChecker;
            }
        }

        /// <summary>Set polling interval (default 10s).</summary>
        public void SetInterval(TimeSpan interval)
        {
            lock (_lock) { _interval = interval <= TimeSpan.Zero ? TimeSpan.FromSeconds(10) : interval; }
            if (_isRunning) RestartTimer();
        }

        /// <summary>Start polling immediately and then every interval.</summary>
        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning) return;
                _isRunning = true;
                _timer = new System.Threading.Timer(async _ => await PollNowSafeAsync(),
                                                    null, TimeSpan.Zero, _interval); // first tick = now
            }
        }

        /// <summary>Stop polling.</summary>
        public void Stop()
        {
            lock (_lock)
            {
                _isRunning = false;
                _timer?.Dispose();
                _timer = null;
            }
        }

        public void Dispose() => Stop();

        /// <summary>Returns true if we have a cached status newer than maxAge.</summary>
        public bool TryGetFresh(TimeSpan maxAge, out ResponseServerStatus status)
        {
            lock (_lock)
            {
                if (_latest is not null && (DateTimeOffset.UtcNow - _lastChecked) <= maxAge)
                {
                    status = _latest;
                    return true;
                }
            }
            status = default!;
            return false;
        }

        /// <summary>Save a status (from outside) and notify listeners.</summary>
        public void Save(ResponseServerStatus status)
        {
            if (status is null) return;

            if (status.CheckedAt == default)
                status.CheckedAt = DateTimeOffset.UtcNow;

            lock (_lock)
            {
                _latest = status;
                _lastChecked = status.CheckedAt;
            }

            StatusChanged?.Invoke(this, _latest);
        }

        /// <summary>Force a poll right now (awaitable).</summary>
        public async Task<ResponseServerStatus> PollNowAsync(CancellationToken ct = default)
        {
            Func<CancellationToken, Task<ResponseServerStatus>> checker;
            lock (_lock) checker = _activeChecker;

            ResponseServerStatus res;
            try
            {
                res = await checker(ct);
                if (res.CheckedAt == default) res.CheckedAt = DateTimeOffset.UtcNow;
            }
            catch
            {
                res = new ResponseServerStatus
                {
                    Status = "DOWN",
                    LatencyMs = -1,
                    CheckedAt = DateTimeOffset.UtcNow
                };
            }

            Save(res);
            return res;
        }

        private async Task PollNowSafeAsync()
        {
            // Timer callbacks must never throw
            try { await PollNowAsync(); }
            catch { /* ignore */ }
        }

        private void RestartTimer()
        {
            // Called under lock
            _timer?.Change(TimeSpan.Zero, _interval);
        }
    }
}

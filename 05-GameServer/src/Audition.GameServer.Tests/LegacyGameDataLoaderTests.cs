using Audition.GameServer.Domain.Models;
using Audition.GameServer.Infrastructure.Configuration;
using Xunit;

namespace Audition.GameServer.Tests;

public sealed class LegacyGameDataLoaderTests
{
    [Fact]
    public async Task LoadAsync_ParsesNoticeHackMissionAndBattleFiles()
    {
        string tempDirectory = Directory.CreateTempSubdirectory().FullName;

        try
        {
            await File.WriteAllTextAsync(Path.Combine(tempDirectory, "NoticeInfo.ini"), "[Notice]\nChannel00 = \"HELLO\"\nChannel01 = \"WORLD\"\n");
            await File.WriteAllTextAsync(Path.Combine(tempDirectory, "HackList.ini"), "[Hack]\nHackCount = 2\nList00 = A.dll\nList01 = B.dll\n");
            await File.WriteAllTextAsync(Path.Combine(tempDirectory, "MissionInfo.ini"), "[MissionInfo]\nMissionCount = 1\nOccurRate = 100\nPeakTime1 = 20\nPeakTime2 = 0\nPeakRate = 100\n\n[Mission000]\nRank = 1\nPoint = 100\nPerpect = 2\nGreat = 3\nCool = 4\nBad = 5\nMiss = 6\nSuccessivePerpect = 7\nFinish = 8\nMultiply = 9\nMessage = BONUS\n");
            await File.WriteAllTextAsync(Path.Combine(tempDirectory, "BattleInfo.ini"), "[BattleInfo]\nBattleCount = 1\nBattleEntryFee = 300\n\n[PerfectPoint]\nSyncCount = 4\nNormalSoloPerfect = 1\nHighSoloPerfect = 2\nNormalSoloStraightPerfect = 3\nHighSoloStraightPerfect = 4\nNormalSyncPerfect = 5\nHighSyncPerfect = 6\nNormalDanceMasterPerfect = 7\nHighDanceMasterPerfect = 8\nNormalDanceMasterStraightPerfect = 9\nHighDanceMasterStraightPerfect = 10\n\n[Attack]\nUserToDanceMaster = 0\nDanceMasterToUser = 50\n\n[BattleInfo000]\nDanceMaster = 0\nGameMode = 2\nMusicCode = song.tbm\nMapCode = 0\nEntryFee = 200\nPrize = 1600\nPerfect = 21\nGreat = 35\nCool = 29\nBad = 15\nStraightPerfect = 4\nStartPerfectGage = 49\n");

            var sut = new LegacyGameDataLoader();

            LegacyGameDataSnapshot snapshot = await sut.LoadAsync(tempDirectory, CancellationToken.None);

            Assert.NotNull(snapshot.Notice);
            Assert.Equal(2, snapshot.Notice!.Channels.Count);
            Assert.NotNull(snapshot.HackList);
            Assert.Equal(2, snapshot.HackList!.Entries.Count);
            Assert.NotNull(snapshot.Mission);
            Assert.Single(snapshot.Mission!.Rules);
            Assert.Equal("BONUS", snapshot.Mission.Rules[0].Message);
            Assert.NotNull(snapshot.Battle);
            Assert.Single(snapshot.Battle!.Stages);
            Assert.Equal("song.tbm", snapshot.Battle.Stages[0].MusicCode);
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task LoadAsync_ReturnsEmpty_WhenDirectoryDoesNotExist()
    {
        var sut = new LegacyGameDataLoader();

        LegacyGameDataSnapshot snapshot = await sut.LoadAsync(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")), CancellationToken.None);

        Assert.Same(LegacyGameDataSnapshot.Empty, snapshot);
    }
}
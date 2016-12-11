using CloudMineServer.API_server.Services;
using CloudMineServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CloudMineServerTest
{
    public class DataChunkExtenstionsTest
    {

        [Fact]
        public void NextNameReturnsSameNameWithPartNumberIncremented()
        {
            // Arrange
            DataChunk dataChunk = new DataChunk { PartName = "test.jpg.part_8.10" };
            // Act
            var result = dataChunk.NextName();
            // Assert
            Assert.Equal("test.jpg.part_9.10", result);
        }
        [Fact]
        public void NextNameReturnsNullWhenPartNumberIsLast()
        {
            // Arrange
            DataChunk dataChunk = new DataChunk { PartName = "test.jpg.part_10.10" };
            // Act
            var result = dataChunk.NextName();
            // Assert
            Assert.Null(result);
        }
        [Fact]
        public void PreviousNameReturnsSameNameWithPartNumberIncremented()
        {
            // Arrange
            DataChunk dataChunk = new DataChunk { PartName = "test.jpg.part_8.10" };
            // Act
            var result = dataChunk.PreviousName();
            // Assert
            Assert.Equal("test.jpg.part_7.10", result);
        }
        [Fact]
        public void PrevioustNameReturnsNullWhenPartNumberIsFirst()
        {
            // Arrange
            DataChunk dataChunk = new DataChunk { PartName = "test.jpg.part_1.10" };
            // Act
            var result = dataChunk.PreviousName();
            // Assert
            Assert.Null(result);
        }
        [Fact]
        public void FirstInSequenceNameReturnsSameNameWithPartNumberOne()
        {
            // Arrange
            DataChunk dataChunk = new DataChunk { PartName = "test.jpg.part_8.10" };
            // Act
            var result = dataChunk.FirstInSequenceName();
            // Assert
            Assert.Equal("test.jpg.part_1.10", result);
        }
        [Fact]
        public void LastInSequenceNameReturnsSameNameWithPartNumberLast()
        {
            // Arrange
            DataChunk dataChunk = new DataChunk { PartName = "test.jpg.part_8.10" };
            // Act
            var result = dataChunk.LastInSequenceName();
            // Assert
            Assert.Equal("test.jpg.part_10.10", result);
        }
        [Fact]
        public void IndexReturnsPartNumberMinusOne()
        {
            // Arrange
            DataChunk dataChunk = new DataChunk { PartName = "test.jpg.part_8.10" };
            // Act
            var result = dataChunk.Index();
            // Assert
            Assert.Equal(7, result);
        }
        [Fact]
        public void NumberOfChunksInSequenceReturnsLastNumberInString()
        {
            // Arrange
            DataChunk dataChunk = new DataChunk { PartName = "test.jpg.part_8.10" };
            // Act
            var result = dataChunk.NumberOfChunksInSequence();
            // Assert
            Assert.Equal(10, result);
        }

    }

}

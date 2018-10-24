﻿/* Copyright 2018-present MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.IO;
using FluentAssertions;
using MongoDB.Driver.Core.TestHelpers;
using Xunit;

namespace MongoDB.Driver.Core.Operations
{
    public class RetryabilityHelperTests
    {
        [Theory]
        [InlineData(1, true)]
        [InlineData((int)ServerErrorCode.HostNotFound, true)]
        [InlineData((int)ServerErrorCode.HostUnreachable, true)]
        [InlineData((int)ServerErrorCode.NetworkTimeout, true)]
        [InlineData((int)ServerErrorCode.SocketException, true)]
        [InlineData((int)ServerErrorCode.ElectionInProgress, true)]
        [InlineData((int)ServerErrorCode.ExceededTimeLimit, true)]
        [InlineData((int)ServerErrorCode.RetryChangeStream, true)]
        [InlineData((int)ServerErrorCode.WriteConcernFailed, true)]
        [InlineData((int)ServerErrorCode.CappedPositionLost, false)]
        [InlineData((int)ServerErrorCode.CursorKilled, false)]
        [InlineData((int)ServerErrorCode.Interrupted, false)]
        public void IsResumableChangeStreamException_should_return_expected_result_using_code(int code, bool expectedResult)
        {
            var exception = CoreExceptionHelper.CreateMongoCommandException(code);

            var result = RetryabilityHelper.IsResumableChangeStreamException(exception);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(typeof(IOException), false)]
        [InlineData(typeof(MongoConnectionException), true)]
        [InlineData(typeof(MongoCursorNotFoundException), true)]
        [InlineData(typeof(MongoNodeIsRecoveringException), true)]
        [InlineData(typeof(MongoNotPrimaryException), true)]
        public void IsResumableChangeStreamException_should_return_expected_result_using_exception_type(Type exceptionType, bool expectedResult)
        {
            var exception = CoreExceptionHelper.CreateException(exceptionType);

            var result = RetryabilityHelper.IsResumableChangeStreamException(exception);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData((int)ServerErrorCode.HostNotFound, true)]
        [InlineData((int)ServerErrorCode.HostUnreachable, true)]
        [InlineData((int)ServerErrorCode.NetworkTimeout, true)]
        [InlineData((int)ServerErrorCode.SocketException, true)]
        [InlineData((int)ServerErrorCode.WriteConcernFailed, true)]
        public void IsRetryableException_should_return_expected_result_using_code(int code, bool expectedResult)
        {
            var exception = CoreExceptionHelper.CreateMongoCommandException(code);

            var result = RetryabilityHelper.IsRetryableWriteException(exception);

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(typeof(IOException), false)]
        [InlineData(typeof(MongoCursorNotFoundException), false)]
        [InlineData(typeof(MongoConnectionException), true)]
        [InlineData(typeof(MongoNodeIsRecoveringException), true)]
        [InlineData(typeof(MongoNotPrimaryException), true)]
        public void IsRetryableException_should_return_expected_result_using_exception_type(Type exceptionType, bool expectedResult)
        {
            var exception = CoreExceptionHelper.CreateException(exceptionType);

            var result = RetryabilityHelper.IsRetryableWriteException(exception);

            result.Should().Be(expectedResult);
        }
    }
}

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Nohros.Ruby;
using Telerik.JustMock;

namespace Nohros.Metrics.Reporting
{
  public class ServiceReporterFactoryTests
  {
    [Test]
    public void ShouldCreatedAnInstanceOfServiceReporter() {
      var factory = new ServiceReporterFactory();
      var options = new Dictionary<string, string> {
        {ServiceReporterFactory.kServerEndpoint, "tcp://127.0.0.1:8520"},
      };
      var reporter = factory.CreatePollingReporter(options);
      Assert.That(reporter, Is.AssignableTo<ServiceReporter>());
    }

    [Test]
    public void ShouldCreateAnInstanceOfDynamicReporter() {
      var host = Mock.Create<IRubyServiceHost>();
      var factory = new ServiceReporterFactory(host);
      var options = new Dictionary<string, string> {
        {ServiceReporterFactory.kServerEndpoint, "tcp://127.0.0.1:8520"},
      };
      var reporter = factory.CreatePollingReporter(options);
      Assert.That(reporter, Is.AssignableTo<DynamicServiceReporter>());
    }

    [Test]
    public void ShouldThrowExceptionWhenRequiredOptionIsMissing() {
      var factory = new ServiceReporterFactory();
      var options = new Dictionary<string, string>();
      try {
        var reporter = factory.CreatePollingReporter(options);
      } catch(Exception e) {
        Assert.Pass("Exception was throwed");
        return;
      }
      Assert.Fail("Exception was not throwed");
    }
  }
}

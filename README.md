# must.metrics

A metrics library for .NET inspired by the [codahale metrics]
library(http://github.com/codahale/metrics) and [Netflix
servo](http://github.com/codahale/metrics).

Must metrics provides a simple interface for exposing and publishing application metrics in C#. The primary goals are:

* **Flexible Publishing**: Once metrics are exposed, it should be easy to regularly poll the metrics and make them available for internal reporting systems, logs, and services like [Datadog](https://www.datadoghq.com/) and [InfluxDB](https://influxdata.com/).

This is already been implemented inside of Nohros Inc and mous of our applications currently use it.

# Project Details

### Documentation

* [GitHub Wiki](https://github.com/nohros/must.metrics/wiki)

### Communication

* For bugs, feedback, questions and discussions please use [GitHub Issues](https://github.com/nohros/must.metrics/issues).

More details can be found on the [Getting Started](https://github.com/nohros/must.metrics/wiki/Getting-Started) page of the wiki.

## Getting Involved
We use the Collective Code Construction Contract (http://rfc.zeromq.org/spec:22).
collaboration model.

Nuget package:

https://www.nuget.org/packages/must.metrics/

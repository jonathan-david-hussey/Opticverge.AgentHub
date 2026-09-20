using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050))
    .AddDatabase("agenthubdb");

var kafka = builder.AddKafka("kafka");
var cache = builder.AddRedis("cache");
var seq = builder.AddSeq("seq");
var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithDataBindMount("../infra/keycloak");

var prometheus = builder.AddContainer("prometheus", "prom/prometheus", "v3.8.0")
    .WithBindMount("../infra/prometheus", "/etc/prometheus", isReadOnly: true)
    .WithHttpEndpoint(port: 9090, targetPort: 9090);

var grafana = builder.AddContainer("grafana", "grafana/grafana", "13.3.0")
    .WithBindMount("../infra/grafana", "/etc/grafana/provisioning", isReadOnly: true)
    .WithHttpEndpoint(port: 3000, targetPort: 3000);

var otelCollector = builder.AddContainer("otelcollector", "otel/opentelemetry-collector-contrib", "0.142.0")
    .WithBindMount("../infra/otelcollector/config.yaml", "/etc/otelcol-contrib/config.yaml", isReadOnly: true)
    .WithHttpEndpoint(port: 4318, targetPort: 4318, name: "otlp-http");

var kafkaUi = builder.AddContainer("kafka-ui", "provectuslabs/kafka-ui", "latest")
    .WithEnvironment("KAFKA_CLUSTERS_0_NAME", "agenthub")
    .WithEnvironment("KAFKA_CLUSTERS_0_BOOTSTRAPSERVERS", "kafka:9092")
    .WithHttpEndpoint(port: 8085, targetPort: 8080);

var flagd = builder.AddContainer("flagd", "ghcr.io/open-feature/flagd", "v0.12.10")
    .WithBindMount("../infra/flagd", "/etc/flagd", isReadOnly: true)
    .WithArgs("start", "--uri", "file:/etc/flagd/flags.json")
    .WithHttpEndpoint(port: 8013, targetPort: 8013);

var k6 = builder.AddContainer("k6", "grafana/k6", "latest")
    .WithBindMount("../infra/k6", "/scripts", isReadOnly: true)
    .WithArgs("run", "/scripts/api-smoke.js");

var mcpInspector = builder.AddContainer("mcp-inspector", "ghcr.io/modelcontextprotocol/inspector", "latest")
    .WithHttpEndpoint(port: 6274, targetPort: 6274);

var api = builder.AddProject<Projects.Opticverge_AgentHub_Api>("api")
    .WithReference(postgres)
    .WithReference(kafka)
    .WithReference(cache)
    .WithReference(seq)
    .WithReference(keycloak)
    .WaitFor(postgres)
    .WaitFor(kafka)
    .WaitFor(cache)
    .WaitFor(keycloak);

var mcp = builder.AddProject<Projects.Opticverge_AgentHub_Mcp>("mcp")
    .WithReference(cache)
    .WithReference(seq)
    .WithReference(keycloak)
    .WaitFor(keycloak);

builder.AddProject<Projects.Opticverge_AgentHub_Worker>("worker")
    .WithReference(postgres)
    .WithReference(kafka)
    .WithReference(cache)
    .WithReference(seq)
    .WaitFor(kafka)
    .WaitFor(cache);

builder.AddProject<Projects.Opticverge_AgentHub_Web>("web")
    .WithReference(api)
    .WithReference(mcp)
    .WithReference(cache)
    .WithReference(keycloak)
    .WithReference(seq)
    .WaitFor(api);

_ = prometheus;
_ = grafana;
_ = otelCollector;
_ = kafkaUi;
_ = flagd;
_ = k6;
_ = mcpInspector;

builder.Build().Run();

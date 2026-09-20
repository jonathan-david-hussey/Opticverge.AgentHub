import http from "k6/http";
import { check } from "k6";

export const options = {
  vus: 1,
  duration: "10s",
  thresholds: {
    http_req_failed: ["rate<0.01"],
    http_req_duration: ["p(95)<500"]
  }
};

export default function () {
  const response = http.get("http://api:8080/");
  check(response, {
    "api root returns 200": (r) => r.status === 200
  });
}

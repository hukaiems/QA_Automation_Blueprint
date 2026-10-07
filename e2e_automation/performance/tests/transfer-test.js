import { check, sleep } from "k6";
import exec from "k6/execution";
import { SharedArray } from "k6/data";
import { executeTransfer } from "../api/transfer.js";
import { login } from "../api/user_auth.js";

import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

const testData = new SharedArray("Transfer Data", function () {
  return JSON.parse(open("../data/transfer-data.json"));
});

export const options = {
  stages: [
    { duration: "15s", target: 3 },
    { duration: "15s", target: 5 },
    { duration: "10s", target: 0 },
  ],
  thresholds: {
    http_req_duration: ["p(95)<8000"],
  },
};

export default function () {
  const currentIndex = exec.scenario.iterationInTest % testData.length;
  const currentRecord = testData[currentIndex];

  const baseUrl = __ENV.BASE_URL || "https://v3.thuvienfpt.com";
  const username = __ENV.APP_USERNAME || "K6_team2_1@fpt.com";
  const password = __ENV.PASSWORD || "Useruseruser1!";

  const loginRes = login(baseUrl, username, password);

  const userToken = loginRes.json("token");
  const userId = loginRes.json("id");

  const requestBody = {
    balanceTransfered: Number(currentRecord.balanceTransfered),
    token: userToken,
    oldPassword: currentRecord.password,
    id: userId,
    from: currentRecord.senderId,
    to: currentRecord.recipientId,
  };

  // Passed the requestBody
  // Execute Request
  const res = executeTransfer(baseUrl, requestBody);

  // DEBUGGING: Print the exact error if the request fails
  if (res.status !== 200) {
    console.log(`FAILED URL: ${res.url}`);
    console.log(`STATUS CODE: ${res.status}`);
    console.log(`RESPONSE BODY: ${res.body}`);
  }

  // Safely check the response without crashing k6
  check(res, {
    "Transfer successful (200)": (r) => r.status === 200,
  });

  sleep(1);
}

export function handleSummary(data) {
  return {
    "performance/TestReports/summary.html": htmlReport(data),
  };
}

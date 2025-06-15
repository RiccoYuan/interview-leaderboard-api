> test info



test suite: `API`

test name: `LeaderBoard Load Test`

session id: `2025-06-15_03.49.16_session_eb63fef9`

> scenario stats



scenario: `update_score_scenario`

  - ok count: `3000`

  - fail count: `0`

  - all data: `0.4` MB

  - duration: `00:00:30`

load simulations:

  - `inject`, rate: `100`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `3000`, ok = `3000`, RPS = `100`|
|latency (ms)|min = `0.34`, mean = `1.69`, max = `10.33`, StdDev = `1.07`|
|latency percentile (ms)|p50 = `1.46`, p75 = `2.09`, p95 = `3.58`, p99 = `6.15`|
|data transfer (KB)|min = `0.126`, mean = `0.128`, max = `0.13`, all = `0.4` MB|
|||
|name|`update_score`|
|request count|all = `3000`, ok = `3000`, RPS = `100`|
|latency (ms)|min = `0.34`, mean = `1.69`, max = `10.32`, StdDev = `1.07`|
|latency percentile (ms)|p50 = `1.46`, p75 = `2.08`, p95 = `3.55`, p99 = `6.14`|
|data transfer (KB)|min = `0.126`, mean = `0.128`, max = `0.13`, all = `0.4` MB|


> status codes for scenario: `update_score_scenario`



|status code|count|message|
|---|---|---|
|OK|3000||


> scenario stats



scenario: `get_leaderboard_scenario`

  - ok count: `6000`

  - fail count: `0`

  - all data: `23.8` MB

  - duration: `00:00:30`

load simulations:

  - `inject`, rate: `200`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `6000`, ok = `6000`, RPS = `200`|
|latency (ms)|min = `0.33`, mean = `1.71`, max = `16.26`, StdDev = `0.98`|
|latency percentile (ms)|p50 = `1.51`, p75 = `2.04`, p95 = `3.38`, p99 = `5.96`|
|data transfer (KB)|min = `0.127`, mean = `4.068`, max = `7.889`, all = `23.8` MB|
|||
|name|`get_leaderboard`|
|request count|all = `6000`, ok = `6000`, RPS = `200`|
|latency (ms)|min = `0.33`, mean = `1.7`, max = `16.26`, StdDev = `0.98`|
|latency percentile (ms)|p50 = `1.5`, p75 = `2.04`, p95 = `3.37`, p99 = `5.92`|
|data transfer (KB)|min = `0.127`, mean = `4.068`, max = `7.889`, all = `23.8` MB|


> status codes for scenario: `get_leaderboard_scenario`



|status code|count|message|
|---|---|---|
|OK|6000||


> scenario stats



scenario: `get_customer_rank_scenario`

  - ok count: `3000`

  - fail count: `0`

  - all data: `1.2` MB

  - duration: `00:00:30`

load simulations:

  - `inject`, rate: `100`, interval: `00:00:01`, during: `00:00:30`

|step|ok stats|
|---|---|
|name|`global information`|
|request count|all = `3000`, ok = `3000`, RPS = `100`|
|latency (ms)|min = `0.34`, mean = `1.76`, max = `9.41`, StdDev = `1.11`|
|latency percentile (ms)|p50 = `1.51`, p75 = `2.13`, p95 = `3.64`, p99 = `6.83`|
|data transfer (KB)|min = `0.127`, mean = `0.42`, max = `0.88`, all = `1.2` MB|
|||
|name|`get_customer_rank`|
|request count|all = `3000`, ok = `3000`, RPS = `100`|
|latency (ms)|min = `0.34`, mean = `1.75`, max = `9.41`, StdDev = `1.11`|
|latency percentile (ms)|p50 = `1.5`, p75 = `2.13`, p95 = `3.64`, p99 = `6.83`|
|data transfer (KB)|min = `0.127`, mean = `0.42`, max = `0.88`, all = `1.2` MB|


> status codes for scenario: `get_customer_rank_scenario`



|status code|count|message|
|---|---|---|
|OK|3000||



import http from 'k6/http';
import { sleep, check } from 'k6';

// export available outside the file 
// options to run the k6 test, k6 find this one first
export const options = {
    vus: 1,
    duration: '5s',
};

export default function () {
    const res = http.get('https://test.k6.io');
    check(res, {
        // r is input parameters (HTTP req)
        'status is 200': (r) => r.status === 200,
    });
    sleep(1);
}
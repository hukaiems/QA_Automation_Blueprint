const axios = require('axios');
const fs = require('fs');
const path = require('path');

const BASE_URL = 'https://v3.thuvienfpt.com';
const REGISTER_URL = `${BASE_URL}/api/users/`;
const OUTPUT_FILE = path.join(__dirname, '../data/transfer-data.json');
const USER_COUNT = 10;

async function generateData() {
    const accounts = [];
    console.log(`Starting generation of ${USER_COUNT} accounts...`);

    for (let i = 1; i <=USER_COUNT; i++) {
        const timeStamp = Date.now();
        const email = `k6_user_${timeStamp}_${i}@fpt.com`;
        const password = `Useruseruser1!`;

        // now try to perform the register api
        try {
            const payload = {
                name: "John Doeee",
                email: email,
                phone: "01008878980",
                postal: "12345",
                addresse: "H",
                password: password
            };

            const response = await axios.post(REGISTER_URL, payload, {
                headers: { 'Content-Type': 'application/json' }
            });
            
            // check response status
            if (response.status === 201) {
                console.log(`✅ Registered: ${email}`);

                
            }
        } catch {}
    }
}
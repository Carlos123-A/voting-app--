from flask import Flask, request, render_template
import redis
import os

app = Flask(__name__)

r = redis.Redis(host='redis', port=6379)

@app.route('/', methods=['GET', 'POST'])
def vote():
    option_a = "Cats"
    option_b = "Dogs"

    if request.method == 'POST':
        vote = request.form['vote']
        r.incr(vote)

    vote_a = r.get('a') or 0
    vote_b = r.get('b') or 0

    return render_template('index.html', option_a=option_a, option_b=option_b, vote=request.form.get('vote'), hostname=os.getenv('HOSTNAME'))

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)

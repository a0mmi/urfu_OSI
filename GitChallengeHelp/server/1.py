import http.server
import ssl
import pathlib

BASE = pathlib.Path(__file__).parent
CERT = str(BASE / "localhost+2.pem")
KEY  = str(BASE / "localhost+2-key.pem")

HOST = "0.0.0.0"
PORT = 4443

class QuietHandler(http.server.SimpleHTTPRequestHandler):
    def log_message(self, format, *args):
        pass

httpd = http.server.HTTPServer((HOST, PORT), QuietHandler)

context = ssl.SSLContext(ssl.PROTOCOL_TLS_SERVER)

context.load_cert_chain(certfile=CERT, keyfile=KEY)

httpd.socket = context.wrap_socket(httpd.socket, server_side=True)

print(f"HTTPS server running on https://{HOST}:{PORT}")
try:
    httpd.serve_forever()
except KeyboardInterrupt:
    print("\nShutting down")
    httpd.server_close()
